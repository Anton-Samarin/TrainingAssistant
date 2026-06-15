import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
import type { Content, TDocumentDefinitions } from 'pdfmake/interfaces';
import type { WeekPlanDto } from '../api/types';
import { dayShort, mealLabels } from '../labels';

pdfMake.addVirtualFileSystem(pdfFonts as Record<string, string>);

function formatWeekStart(iso: string) {
  return new Date(iso).toLocaleDateString('ru-RU', { day: 'numeric', month: 'long', year: 'numeric' });
}

function mealSection(nutrition: WeekPlanDto['nutritionDays'][number]): Content[] {
  const blocks: Content[] = [
    {
      text: `${nutrition.targetCalories} ккал · Б ${nutrition.targetProteinG} · Ж ${nutrition.targetFatG} · У ${nutrition.targetCarbsG}`,
      style: 'macro',
      margin: [0, 0, 0, 6],
    },
  ];

  for (const meal of nutrition.meals) {
    const primary = meal.items.filter((i) => !i.isAlternative);
    const alternatives = meal.items.filter((i) => i.isAlternative);
    const lines: Content[] = primary.map((item) => ({
      text: `• ${item.foodName} — ${item.grams} г, ${item.calories} ккал`,
      margin: [8, 1, 0, 1] as [number, number, number, number],
    }));

    if (alternatives.length > 0) {
      lines.push({
        text: 'или:',
        italics: true,
        color: '#6f6a60',
        margin: [8, 4, 0, 2] as [number, number, number, number],
      });
      alternatives.forEach((item) => {
        lines.push({
          text: `• ${item.foodName} — ${item.grams} г`,
          color: '#6f6a60',
          margin: [16, 1, 0, 1] as [number, number, number, number],
        });
      });
    }

    blocks.push({
      stack: [{ text: mealLabels[meal.mealType] ?? meal.mealType, style: 'mealTitle' }, ...lines],
      margin: [0, 0, 0, 8] as [number, number, number, number],
    });
  }

  return blocks;
}

function trainingSection(training: WeekPlanDto['trainingDays'][number]): Content[] {
  if (training.isRestDay) {
    return [{ text: 'День отдыха', italics: true, color: '#6f6a60' }];
  }

  const blocks: Content[] = [];
  if (training.focus) {
    blocks.push({ text: training.focus, style: 'macro', margin: [0, 0, 0, 6] });
  }

  training.exercises.forEach((ex, idx) => {
    const meta = `${ex.sets}×${ex.reps}${ex.restSec ? ` · отдых ${ex.restSec} с` : ''}`;
    blocks.push({
      columns: [
        { width: 20, text: String(idx + 1).padStart(2, '0'), style: 'exNum' },
        {
          width: '*',
          stack: [
            { text: ex.name, bold: true },
            { text: meta, style: 'exMeta' },
            ...(ex.notes ? [{ text: ex.notes, style: 'exMeta' }] : []),
          ],
        },
      ],
      margin: [0, 0, 0, 6] as [number, number, number, number],
    });
  });

  return blocks;
}

export function exportWeekPlanPdf(plan: WeekPlanDto, userName?: string) {
  const content: Content[] = [
    { text: 'TrainingAssistant', style: 'title' },
    { text: `План на неделю с ${formatWeekStart(plan.weekStart)}`, style: 'subtitle' },
  ];

  if (userName) {
    content.push({ text: userName, style: 'muted', margin: [0, 0, 0, 10] });
  }

  if (plan.programType) {
    const conf =
      plan.programConfidence != null ? ` (уверенность ${Math.round(plan.programConfidence * 100)}%)` : '';
    content.push({ text: `Программа: ${plan.programType}${conf}`, style: 'muted', margin: [0, 0, 0, 10] });
  }

  if (plan.health) {
    content.push({
      text: `ИМТ ${plan.health.bmi} — ${plan.health.bmiCategory}`,
      style: 'muted',
      margin: [0, 0, 0, 14],
    });
  }

  for (let i = 1; i <= 7; i++) {
    const nutrition = plan.nutritionDays.find((d) => d.dayIndex === i);
    const training = plan.trainingDays.find((d) => d.dayIndex === i);
    const dayLabel = dayShort[i - 1];
    const dayName = training?.dayName ?? `День ${i}`;

    content.push({
      text: `${dayLabel} · ${dayName}`,
      style: 'dayTitle',
      pageBreak: i > 1 ? 'before' : undefined,
      margin: [0, i === 1 ? 0 : 0, 0, 8],
    });

    content.push({
      columns: [
        {
          width: '*',
          stack: [{ text: 'Питание', style: 'sectionTitle' }, ...(nutrition ? mealSection(nutrition) : [{ text: '—', style: 'muted' }])],
        },
        {
          width: '*',
          stack: [
            { text: 'Тренировка', style: 'sectionTitle' },
            ...(training ? trainingSection(training) : [{ text: '—', style: 'muted' }]),
          ],
        },
      ],
      columnGap: 20,
    });
  }

  content.push({
    text: `Сформировано ${new Date().toLocaleString('ru-RU')}`,
    style: 'footer',
    margin: [0, 24, 0, 0],
  });

  const docDefinition: TDocumentDefinitions = {
    content,
    defaultStyle: {
      font: 'Roboto',
      fontSize: 10,
      lineHeight: 1.35,
    },
    styles: {
      title: { fontSize: 18, bold: true, color: '#c45c26', margin: [0, 0, 0, 4] },
      subtitle: { fontSize: 13, bold: true, margin: [0, 0, 0, 6] },
      muted: { fontSize: 9, color: '#6f6a60' },
      dayTitle: { fontSize: 12, bold: true, color: '#c45c26' },
      sectionTitle: { fontSize: 10, bold: true, margin: [0, 0, 0, 6] },
      mealTitle: { fontSize: 9, bold: true, color: '#c45c26', margin: [0, 0, 0, 3] },
      macro: { fontSize: 9, color: '#6f6a60' },
      exNum: { fontSize: 9, bold: true, color: '#c45c26' },
      exMeta: { fontSize: 9, color: '#6f6a60', margin: [0, 1, 0, 0] },
      footer: { fontSize: 8, color: '#6f6a60', alignment: 'right' },
    },
    pageMargins: [40, 48, 40, 48],
  };

  const fileName = `plan-${plan.weekStart.slice(0, 10)}.pdf`;
  pdfMake.createPdf(docDefinition).download(fileName);
}

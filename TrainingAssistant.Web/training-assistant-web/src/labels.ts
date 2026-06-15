import { FitnessLevel, TrainingFocus, UserGoal, UserSex } from './api/types';

export const goalLabels: Record<UserGoal, string> = {
  [UserGoal.LoseWeight]: 'Похудение',
  [UserGoal.GainMuscle]: 'Набор массы',
  [UserGoal.Maintain]: 'Поддержание',
};

export const sexLabels: Record<UserSex, string> = {
  [UserSex.Male]: 'Мужской',
  [UserSex.Female]: 'Женский',
};

export const trainingFocusLabels: Record<TrainingFocus, string> = {
  [TrainingFocus.Strength]: 'Сила',
  [TrainingFocus.Endurance]: 'Выносливость',
  [TrainingFocus.Mixed]: 'Смешанный',
};

export const levelLabels: Record<FitnessLevel, string> = {
  [FitnessLevel.Beginner]: 'Новичок',
  [FitnessLevel.Intermediate]: 'Средний',
  [FitnessLevel.Advanced]: 'Продвинутый',
};

export const mealLabels: Record<string, string> = {
  Breakfast: 'Завтрак',
  Lunch: 'Обед',
  Dinner: 'Ужин',
  Snack: 'Перекус',
};

export const dayShort = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];

/** Повседневная активность вне тренировок — влияет на расчёт калорий (TDEE) */
export const activityLevels = [
  {
    level: 1,
    title: 'Минимальная',
    description: 'Сидячая работа, почти нет прогулок и быта на ногах.',
    hint: 'Офис, дом, мало движения в течение дня.',
  },
  {
    level: 2,
    title: 'Низкая',
    description: 'Лёгкая активность: прогулки, немного ходьбы по дому или до работы.',
    hint: '1–3 тыс. шагов в день сверх привычного.',
  },
  {
    level: 3,
    title: 'Умеренная',
    description: 'Средний быт: стоишь/ходишь часть дня, регулярные прогулки.',
    hint: 'Подходит большинству с обычным ритмом жизни.',
  },
  {
    level: 4,
    title: 'Высокая',
    description: 'Активная работа или много движения: подъёмы, долгие прогулки, физический труд.',
    hint: 'Не путать с интенсивностью тренировок — это про будни.',
  },
  {
    level: 5,
    title: 'Очень высокая',
    description: 'Постоянная физическая нагрузка в течение дня, тяжёлый труд или спорт вне зала.',
    hint: 'Курьеры, строители, официанты и похожие профили.',
  },
] as const;

export const equipmentOptions = [
  { id: 'dumbbells', label: 'Гантели' },
  { id: 'mat', label: 'Коврик' },
  { id: 'pullup_bar', label: 'Турник' },
  { id: 'barbell', label: 'Штанга' },
  { id: 'gym', label: 'Зал' },
];

export const injuryOptions = [
  { id: 'knee', label: 'Колено' },
  { id: 'back', label: 'Спина' },
  { id: 'shoulder', label: 'Плечо' },
];

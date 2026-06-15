from __future__ import annotations

from typing import Optional

from fastapi import Header, HTTPException, status

from app.config import settings


def verify_api_key(x_api_key: Optional[str] = Header(default=None)) -> None:
    if not x_api_key or x_api_key != settings.api_key:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid or missing API key",
        )

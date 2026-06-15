from fastapi import APIRouter

from app.api.v1 import health, training

api_router = APIRouter()
api_router.include_router(health.router, tags=["health"])
api_router.include_router(training.router, tags=["training"])

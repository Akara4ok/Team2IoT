import os


def try_parse(type, value: str):
    try:
        return type(value)
    except Exception:
        return None


import json
import logging
from typing import List

import pydantic_core
import requests

from app.entities.processed_agent_data import ProcessedAgentData
from app.interfaces.store_gateway import StoreGateway

from config import (
    STORE_API_UPLOAD_URL
)


class StoreApiAdapter(StoreGateway):
    def __init__(self, api_base_url):
        self.api_base_url = api_base_url

    def save_data(self, processed_agent_data_batch: List[ProcessedAgentData]):
        try:
            json_data_list = [model.json() for model in processed_agent_data_batch]
            json_res = "[" + ",".join(json_data_list) + "]"

            response = requests.post(
                STORE_API_UPLOAD_URL,
                data=json_res
            )
            response.raise_for_status()
            return response.status_code == 200
        except requests.RequestException as e:
            logging.error(f"Error saving processed agent data: {e}")
            return False
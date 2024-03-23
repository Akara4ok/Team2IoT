from app.entities.agent_data import AgentData
from app.entities.processed_agent_data import ProcessedAgentData
import os
import numpy as np
from scipy.signal import find_peaks
import logging
MAX_PREV_VALUES_COUNT = 250

class DataProcessor:
    def __init__(self) -> None:
        self.prev_values: list[AgentData] = []
        self.shown = False
        self.save = False
        
    def add_value(self, agent_data: AgentData):
        self.prev_values.append(agent_data)
        
        if(not self.save):
            count = min(MAX_PREV_VALUES_COUNT, len(self.prev_values))
            self.prev_values = self.prev_values[-count:]
            return
        
        
        if((len(self.prev_values) == MAX_PREV_VALUES_COUNT) and not self.shown):
            self.shown = True
            x = [6000 * agent.timestamp.minute + 100 * agent.timestamp.second + agent.timestamp.microsecond // 1000 for agent in self.prev_values]
            y1 = [int(agent.accelerometer.x) for agent in self.prev_values]
            y2 = [int(agent.accelerometer.y) for agent in self.prev_values]
            y3 = [int(agent.accelerometer.z) for agent in self.prev_values]
            
            if not os.path.exists("/result"):
                os.makedirs("/result")
            
            np.save("/result/testx.npy", x)
            for i, y in enumerate([y1, y2, y3]):
                path = "/result/test"+str(i)
                np.save(path + ".npy", y)
                
    def analyze(self):
        y = np.asarray([int(agent.accelerometer.z) for agent in self.prev_values])
        peaks, _ = find_peaks(-y, prominence=10000, distance=50)
        MIDDLE_PEAKS = 2
        DANGER_VELOCITY = -10000
        
        x_reduced = np.linspace(0, len(self.prev_values) - 1, num=len(self.prev_values) - 1).astype(int)
        x_reduced = x_reduced[-(MAX_PREV_VALUES_COUNT//3):]
        
        if(np.any(y[x_reduced] < DANGER_VELOCITY) and len(peaks) >= 1):
            return "Poor"

        if((len(peaks) >= MIDDLE_PEAKS) and not(np.any(y[x_reduced] < DANGER_VELOCITY) and len(peaks) >= 1)):
            return "Normal"

        if((len(peaks) < MIDDLE_PEAKS) and not(np.any(y[x_reduced] < DANGER_VELOCITY) and len(peaks) >= 1)):
            return "Good"
        
        return 

processor = DataProcessor() 

def process_agent_data(
    agent_data: AgentData,
) -> ProcessedAgentData:
    """
    Process agent data and classify the state of the road surface.
    Parameters:
        agent_data (AgentData): Agent data that containing accelerometer, GPS, and timestamp.
    Returns:
        processed_data_batch (ProcessedAgentData): Processed data containing the classified state of the road surface and agent data.
    """
    
    processor.add_value(agent_data)
    road_state = processor.analyze()
    
    return ProcessedAgentData(road_state=road_state, agent_data=agent_data)
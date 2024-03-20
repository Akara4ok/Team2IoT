from app.entities.agent_data import AgentData
from app.entities.processed_agent_data import ProcessedAgentData


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
    # Implement it
    z_acceleration = agent_data.accelerometer.z

    if z_acceleration < 0:
        road_state = "Poor"
    elif 0 <= z_acceleration <= 7000:
        road_state = "Normal"
    else:
        road_state = "Good"
    
    return ProcessedAgentData(road_state=road_state, agent_data=agent_data)
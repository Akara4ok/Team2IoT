from file_datasource import FileDatasource

def run():
    datasource = FileDatasource("data/accelerometer.csv", "data/gps.csv")


if __name__ == "__main__":
    run()

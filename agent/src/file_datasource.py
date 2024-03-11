from csv import reader
from datetime import datetime
from domain.accelerometer import Accelerometer
from domain.gps import Gps
from domain.aggregated_data import AggregatedData
import config
from itertools import cycle

class FileDatasource:
    def __init__(
        self,
        accelerometer_filename: str,
        gps_filename: str,
    ) -> None:
        self.accelerometer_filename = accelerometer_filename
        self.gps_filename = gps_filename

    def read(self) -> AggregatedData:
        if self.accelReader is not None:
            try:
                if not self.headersAccelSkipped:
                    next(self.accelReader)  # Skip the headers
                    self.headersAccelSkipped = True

                if not self.headersGpsSkipped:
                    next(self.gpsReader)  # Skip the headers
                    self.headersGpsSkipped = True

                rowAccel = next(self.accelReader, None)
                rowGps = next(self.gpsReader, None)

                if not rowAccel or rowAccel is None:
                    self.accelFile.seek(0)  # Go back to the start of the file
                    next(self.accelReader)  # Skip headers again
                    rowAccel = next(self.accelReader)  # Read the first row
                    self.headersAccelSkipped = True


                if not rowGps or rowGps is None:
                    self.gpsFile.seek(0)  # Go back to the start of the file
                    next(self.gpsReader)  # Skip headers again
                    rowGps = next(self.gpsReader)  # Read the first row
                    self.headersGpsSkipped = True

                return AggregatedData(
                        Accelerometer(rowAccel[0], rowAccel[1], rowAccel[2]),
                        Gps(rowGps[0], rowGps[1]),
                        datetime.now(),
                        0,
                        )
            except Exception as e:
                print(f"An error occurred while reading the file: {e}")
                return None
        else:
            print("File is not open. Please call startReading first.")
            return None

    def startReading(self, *args, **kwargs):
        [isAccelOpen, accelFile, accelReader] = self.openFile(self.accelerometer_filename)
        self.accelFile = accelFile
        self.accelReader = accelReader

        [isGpsOpen, gpsFile, gpsReader] = self.openFile(self.gps_filename)
        self.gpsFile = gpsFile
        self.gpsReader = gpsReader

        isFilesOpened = isAccelOpen and isGpsOpen

        if not isFilesOpened:
            raise Exception(f"Files not opened") 
        
        self.headersAccelSkipped = False
        self.headersGpsSkipped = False

    def openFile(self, filename):
        try:
            file = open(filename, mode='r', newline='')
            readerOpened = reader(file)
            print(f"Opened file {filename} for reading.")
            return [True, file, readerOpened]
        except FileNotFoundError:
            print(f"File {filename} not found.")
        except Exception as e:
            print(f"An error occurred while opening the file: {e}")

        return [False, None, None]

    def stopReading(self, *args, **kwargs):
        self.closeFile(self.accelFile)
        self.closeFile(self.gpsFile)

                
    def closeFile(self, file):
        if self.file is not None and not file.closed:
            file.close()
        else:
            print("File is already closed.")

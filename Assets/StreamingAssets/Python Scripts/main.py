import sqlite3

#Setup SQLite
conn = sqlite3.connect('../database.db')
c = conn.cursor()

#Create Sample's Records Table
def createSampleRecordsTable(sampleName, sampleId):
    c.execute(
        '''CREATE TABLE {sampleName} AS 
        SELECT * from Records 
        WHERE SampleId = {sampleId}'''
        .format(sampleName = sampleName, sampleId = sampleId)
        )
    print(sampleId)

sampleName = "sample"

for i in range (1,720):
    sampleName = sampleName + str(i)
    createSampleRecordsTable(sampleName,i)
    sampleName = "sample"
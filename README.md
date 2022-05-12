# BioVr

## Author Contact Information


samuel.yeaman@ucalgary.ca
john.kohout@unity3d.com

### Assistant
khoa.nguyentrandang@ucalgary.ca
anh.nguyen5@ucalgary.ca

## Usage and license information

If you use or are inspired by code in this repository please cite the following work or contact me about how to cite. Please also see [license information](LICENSE).

---

# BioVR

Brief blurb about the motivation and structure of this repository, including outcomes (if applicable).

---

## Environment setup

Unity Editor version `2020.3.11f1`

### Database
The original database is created with:

- `CREATE TABLE Populations (PopulationId integer PRIMARY KEY,PopulationName text  UNIQUE NOT NULL)`
- `CREATE TABLE Records (Position integer,SampleId integer,GenotypeId integer,FOREIGN KEY (SampleId) REFERENCES Samples (SampleId))`
- `CREATE TABLE Samples (SampleId integer PRIMARY KEY,SampleName text UNIQUE NOT NULL,Altitude real,Latitude real,Longitude real)`
- `CREATE TABLE SamplesToPopulations (PopulationId integer,FOREIGN KEY (PopulationId) REFERENCES Populations (PopulationId)FOREIGN KEY (SampleId) REFERENCES Samples (SampleId))`

But to run, use `main.py` from `BioVR\BioInfoVR\Assets\StreamingAssets\Python Scripts` to run efficiently.
 
## Usage

- Hit play button in Unity Editor
- Click on a population (cone shaped) object
- Graph of the according population appears

[LICENSE](LICENSE) - our license information

## Details:

The database used contains:

- 65 populations
- 719 samples
- 53821464 records

Each population contains 1-10 samples. And one sample contains 74856 records. Thus we want to find a solution that can graph 10 * 74856 records for each population to achieve the same outcome as in : https://jimw91.shinyapps.io/genotype_plot_demo/.

Techniques we used, in order:

- Draw records in group of the same genotype. The result is drawn in 6 minutes.
- Divide database into non-SQL, as a proof of concept, to enhance reading from the database over writing to the database. The runtime reduced to 1 minute 6 seconds.
- Use multiple threads to read from the database, using the Jobs System by Unity. The result is shown in 50 seconds.



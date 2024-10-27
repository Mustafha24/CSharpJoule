using System;

namespace CSharp_Joules
{
    public class Measure_Energy{
       private const string RAPL_DIR="/sys/class/powercap/intel-rapl";
       private List<int> socketIdsList= new List<int>();
       private List<int> packageDomainsList= new List<int>();
       private List<int> dramDomainsList= new List<int>();

        public Measure_Energy(){
            socketIdsList=GetSocketIdsList();
            packageDomainsList=GetPackageDomainsList();
            dramDomainsList=GetDramDomainsList();
        }

        private List<int> GetSocketIdsList(){
            List<int> list=new List<int>();
            int socketId=0;
            string name=$"{RAPL_DIR}/intel-rapl:{socketId}";
            while(Directory.Exists(name)){
                list.Add(socketId);
                socketId+=1;
                name=$"{RAPL_DIR}/intel-rapl:{socketId}";
            }
            if(list.Count==0){
                Console.WriteLine("Machine Not Supported");
                Environment.Exit(0);
            }
            return list;

        }
        private List<int> GetPackageDomainsList(){
            List<int> list=new List<int>();
            foreach (var socketId in socketIdsList)
            {
                string domainNameFile=$"{RAPL_DIR}/intel-rapl:{socketId}/name";
                if(File.Exists(domainNameFile)){
                    string r1=File.ReadAllLines(domainNameFile)[0].Trim();
                    string packageDomain=$"package-{socketId}";
                    if(packageDomain.Equals(r1)){
                        list.Add(socketId);
                    }   
                }
            }
            return list;
        }
        private List<int> GetDramDomainsList(){
            List<int> list=new List<int>();
            string[] dramVars={"dram","core"};
            foreach(var socketId in socketIdsList){
                int domainId=0;
                while(true){
                    string domainNameFile=$"{RAPL_DIR}/intel-rapl:{socketId}/intel-rapl:{socketId}:{domainId}/name";
                    if(File.Exists(domainNameFile)){
                        string r1=File.ReadAllLines(domainNameFile)[0].Trim();
                        //Console.WriteLine(r1);
                        if(Array.Exists(dramVars,element=>element.Equals(r1))){
                            list.Add(socketId);
                        }
                        domainId++;
                    }
                    else{
                        break;
                    }
                }
            }
            return list;
        
        }

        public (List<long> EnergiesDram,List<long> EnergiesPackage) GetEnergyTrace(){

            List<long> energiesDram=new List<long>();
            List<long> energiesPackage=new List<long>();

            foreach (var socketId in dramDomainsList)
            {
                int domainId=0;
                string domainNameFile=$"{RAPL_DIR}/intel-rapl:{socketId}/intel-rapl:{socketId}:{domainId}/name";
                if(File.Exists(domainNameFile)){

                    string r1=File.ReadAllLines(domainNameFile)[0].Trim();
                    if(r1.Equals("dram") || r1.Equals("core")){

                        string valueDir=$"{RAPL_DIR}/intel-rapl:{socketId}/intel-rapl:{socketId}:{domainId}/energy_uj";

                        if(File.Exists(valueDir)){
                             if (long.TryParse(File.ReadAllLines(valueDir)[0].Trim(), out long energy))
                            {
                                energiesDram.Add(energy);
                            }
                        }
                    }
                }
            }


            foreach(var socketId in packageDomainsList){
                string packageDomainDir=$"{RAPL_DIR}/intel-rapl:{socketId}/energy_uj";
                if(File.Exists(packageDomainDir)){
                     if (long.TryParse(File.ReadAllLines(packageDomainDir)[0].Trim(), out long energy))
                    {
                        energiesPackage.Add(energy);
                    }
                }
            }
            return (energiesDram,energiesPackage);
        }
    }
}

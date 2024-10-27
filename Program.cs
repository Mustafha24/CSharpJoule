using System;

namespace CSharp_Joules
{
    class Program{
        public static void Main(String[] args){
            Display_Energy de=new Display_Energy();

            de.MeasureEnergy(Test_Snippet,"Test_Snippet");
            Console.WriteLine("Energy Measurement Completed");

            static void Test_Snippet(){
                for(int i=0;i<100;i++){
                    for(int j=0;j<100;i++){
                    }
                }
            }


        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using MLApp;
namespace FuzzyEntropy_antColony
{
    class InformationGain
    {
        private byte[,] Data;
        private byte[,] Class;
        private int[] SUM_Term;
        private byte Class_count;
        private byte Type_Dataset;
        private float[] P;
        private float[] C;
        private float[,] P_C;
        private double[,] Gain;
        private float[] H_Class;
        string ds = "";
        
        public void GetData(byte[,] Data_t, byte[,] Class_t, byte Type_Dataset_t)
        {
            this.Data = Data_t;
            this.Class = Class_t;

            this.Type_Dataset = Type_Dataset_t;
            
            if (Type_Dataset == 0)
            {
                ds = "Reuters";
                Class_count = 65;
            }
            else if (Type_Dataset == 1)
            {
                ds = "20newsgroup";
                Class_count = 20;
            }
        }
        public void Calculate_IG(string saveResult_url)
        {
            int sum_p;
            int sum_c;
            MLAppClass matlab = new MLAppClass();
            P = new float[Data.GetLength(1)];
            C = new float[Class_count];
            P_C = new float[Class_count, Data.GetLength(1)];
            SUM_Term = new int[Data.GetLength(1)];
            H_Class = new float[Class_count];
            Gain = new double[3,Data.GetLength(1)];
            for (int i=0;i<Data.GetLength(1);i++)
            {
                sum_p = 0;
                for (int j=0; j<Data.GetLength(0);j++)
                {
                    if(Data[j,i]>0)
                    {
                        sum_p++;
                        SUM_Term[i] = SUM_Term[i] + Data[j, i];
                    }
                }
                P[i] = (float)((float)sum_p / (float)Data.GetLength(0));
            }
            for (int i = 0; i < Class_count; i++)
            {
                sum_c = 0;
                for (int j = 0; j < Class.GetLength(0); j++)
                {
                    if (Class[j, 0] == (i+1))
                    {
                        sum_c++;
                    }
                }
                C[i] = (float)((float)sum_c / (float)Data.GetLength(0));
                if(C[i]!=0)
                    H_Class[i] = (float)(C[i] * Math.Log(C[i]));
            }
            for (int c = 0; c < Class_count; c++)
            {
                for (int i = 0; i < Data.GetLength(1); i++)
                {
                    sum_p = 0;
                    for (int j = 0; j < Data.GetLength(0); j++)
                    {
                        if (Data[j, i] > 0 && Class[j,0] == (c+1))
                        {
                            sum_p++;
                        }
                    }
                    P_C[c,i] = (float)((float)sum_p / (float)Data.GetLength(0))/(float)P[i];
                }
            }
            double gain = 0;
            
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                if(i>714)
                {
                    gain = 0;
                }
                gain = 0;
                for (int c = 0; c < Class_count; c++)
                {
                    if(P_C[c, i]>0 && (1 - P_C[c, i])>0)
                        gain= gain+((-1 * H_Class[c]) + (P[i] * (P_C[c, i] * Math.Log(P_C[c, i]))) + ((1 - P[i]) * ((1 - P_C[c, i]) * Math.Log(1 - P_C[c, i]))));
                }
                Gain[0,i] = gain;
                Gain[1, i] = SUM_Term[i];
                Gain[2, i] = (i + 1);

            }
            matlab.PutWorkspaceData("Gain", "base", Gain);
            matlab.Execute(@"Gain=(Gain')");
            matlab.Execute(@"Gain=sortrows(Gain,-1)");
            matlab.Execute(@"Gain=(Gain')");
            matlab.Execute(@"save('" + saveResult_url + "Gain_" + ds + ".mat');");
        }
    }
}



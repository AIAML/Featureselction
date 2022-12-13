  class GiniIndex
    {
        private byte[,] Data;
        private byte[,] Class;
        private int[] SUM_Term;
        private byte Class_count;
        private byte Type_Dataset;
        private float[] P;
        private float[] C;
        private float[,] P_C;
        private float[,] C_P;
        private double[,] Gini;
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
        public void Calculate_Gini(string saveResult_url)
        {
            int sum_p;
            int sum_c;
            MLAppClass matlab = new MLAppClass();
            P = new float[Data.GetLength(1)];
            C = new float[Class_count];
            P_C = new float[Class_count, Data.GetLength(1)];
            C_P = new float[Class_count, Data.GetLength(1)];
            SUM_Term = new int[Data.GetLength(1)];
            H_Class = new float[Class_count];
            Gini = new double[3,Data.GetLength(1)];
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
            for (int c = 0; c < Class_count; c++)
            {
                for (int i = 0; i < Data.GetLength(1); i++)
                {
                    sum_p = 0;
                    for (int j = 0; j < Data.GetLength(0); j++)
                    {
                        if (Data[j, i] > 0 && Class[j, 0] == (c + 1))
                        {
                            sum_p++;
                        }
                    }
                    C_P[c, i] = (float)((float)sum_p / (float)Data.GetLength(0)) / (float)C[c];
                }
            }
            double gini = 0;
            
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                gini = 0;
                for (int c = 0; c < Class_count; c++)
                {
                    if (P_C[c, i] > 0 && C_P[c, i] > 0)
                        gini = gini + (Math.Pow(C_P[c, i],2) * Math.Pow(P_C[c, i], 2));
                }
                Gini[0,i] = gini;
                Gini[1, i] = SUM_Term[i];
                Gini[2, i] = (i + 1);
            }
            matlab.PutWorkspaceData("Gini", "base", Gini);
            matlab.Execute(@"Gini=(Gini')");
            matlab.Execute(@"Gini=sortrows(Gini,-1)");
            matlab.Execute(@"Gini=(Gini')");
            matlab.Execute(@"save('" + saveResult_url + "Gini_" + ds + ".mat');");
        }
    }

  class TS
    {
        private byte[,] Data;
        private byte[,] Class;
        private int[] SUM_Term;
        private int[] SUM_NOT_Term;
        
        private byte Class_count;
        private byte Type_Dataset;
        private float[] T;
        private float[] NT;
        private float[] C;
        private float[] NC;
        private float[,] C_T;
        private float[,] T_NC;
        private float[,] NT_C;
        private double[,] TS_;
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
        public void Calculate_TS(string saveResult_url)
        {
            int sum_p;
            int sum_c;
            MLAppClass matlab = new MLAppClass();
            T = new float[Data.GetLength(1)];
            NT = new float[Data.GetLength(1)];
            C = new float[Class_count];
            NC = new float[Class_count];
            C_T = new float[Class_count, Data.GetLength(1)];
            T_NC = new float[Class_count, Data.GetLength(1)];
            NT_C = new float[Class_count, Data.GetLength(1)];
            SUM_Term = new int[Data.GetLength(1)];
            SUM_NOT_Term = new int[Data.GetLength(1)];
            H_Class = new float[Class_count];
            TS_ = new double[3,Data.GetLength(1)];
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
                T[i] = (float)((float)sum_p / (float)Data.GetLength(0));
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
                    C_T[c, i] = (float)((float)sum_p / (float)Data.GetLength(0)) / (float)T[i];
                }
            }

            //=================================================================================================================================================================

            double TS = 0;

            for (int i = 0; i < Data.GetLength(1); i++)
            {
                TS = 0;
                for (int c = 0; c < Class_count; c++)
                {
                    if (C_T[c, i] > 0 && NT_C[c, i] > 0 && T_NC[c, i] > 0)
                        TS = TS + C_T[c, i];
                }
                TS_[0,i] = T[i];
                TS_[1, i] = SUM_Term[i];
                TS_[2, i] = (i + 1);
            }
            matlab.PutWorkspaceData("TS", "base", TS_);
            matlab.Execute(@"TS=(TS')");
            matlab.Execute(@"TS=sortrows(TS,-1)");
            matlab.Execute(@"TS=(TS')");
            matlab.Execute(@"save('" + saveResult_url + "TS_" + ds + ".mat');");
        }

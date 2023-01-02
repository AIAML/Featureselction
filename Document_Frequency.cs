 class DF
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
        private double[,] DF_;
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
        public void Calculate_DF(string saveResult_url)
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
            DF_ = new double[3,Data.GetLength(1)];
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
                T[i] = sum_p;
            }


            //=================================================================================================================================================================
            
            for (int i = 0; i < Data.GetLength(1); i++)
            {                
                DF_[0,i] = T[i];
                DF_[1, i] = SUM_Term[i];
                DF_[2, i] = (i + 1);
            }
            matlab.PutWorkspaceData("DF", "base", DF_);
            matlab.Execute(@"DF=(DF')");
            matlab.Execute(@"DF=sortrows(DF,-1)");
            matlab.Execute(@"DF=(DF')");
            matlab.Execute(@"save('" + saveResult_url + "DF_" + ds + ".mat');");
        }
    }

    class MI
    {
        
        private byte[,] Data;
        private byte[,] Class;
        private byte Class_count;
        private double[,] MI_Result;
        private double[,] MI_R;
        private byte Type_Dataset;
        private int[,] A;
        private int[,] B;
        private int[,] E;
        private int[,] D;
        private int[] SUM_Term;
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
            this.MI_Result = new double[Data.GetLength(1),Class_count];
            this.MI_R = new double[3,Data.GetLength(1)];
            this.SUM_Term = new int[Data.GetLength(1)];
        }
        public void Calculate_MI2(string saveResult_url)
        {

            MLAppClass matlab = new MLAppClass();
            for (int i = 0; i < Data.GetLength(1); i++)
            {
                for (int j = 0; j < Data.GetLength(0); j++)
                {
                    if (Data[j, i] > 0)
                    {
                        SUM_Term[i] = SUM_Term[i] + Data[j, i];
                    }
                }
            }

            byte a_count,b_count,e_count,d_count;

                A = new int[Data.GetLength(1), Class_count];
                B = new int[Data.GetLength(1), Class_count];
                E = new int[Data.GetLength(1), Class_count];
                D = new int[Data.GetLength(1), Class_count];
            
            for (int features = 0; features < Data.GetLength(1) ; features++)
            {
                for (int k = 1; k <= Class_count; k++)
                {

                    a_count = 0;
                    b_count = 0;
                    e_count = 0;
                    d_count = 0;
                    for (int row = 0; row < Data.GetLength(0); row++)
                    {
                        if (Data[row, features] >0 && Class[row, 0] == k)
                        {
                            a_count++;
                            A[features, k - 1] = a_count;
                        }
                        else if (Data[row, features] > 0 && Class[row, 0] != k)
                        {
                            b_count++;
                            B[features, k - 1] = b_count;
                        }
                        else if (Data[row, features] == 0 && Class[row, 0] == k)
                        {
                            e_count++;
                            E[features, k - 1] =e_count;
                        }
                    }// end of for (int row = 0; row < Data.GetLength(0); row++)
                }//end of for (int k = 1; k <= Class_count; k++)
            }//end of (int features = 0; features < Data.GetLength(1) ; features++)
            int N = Data.GetLength(0);
            int AA, BB, EE, DD;
            double sum_ = 0;
            for (int features = 0; features < Data.GetLength(1); features++)
            {
                sum_ = 0;
                for (int k = 1; k <= Class_count; k++)
                {
                    AA = A[features, k - 1];
                    BB = B[features, k - 1];
                    EE = E[features, k - 1];
                    if((AA * N) >0 && ((AA + EE) * (AA + BB))>0)
                    {
                        MI_Result[features, k - 1] = Math.Log((AA * N) / (AA + EE) * (AA + BB));
                        sum_ = sum_ + MI_Result[features, k - 1];
                    }
                    MI_R[0, features] = sum_;
                    MI_R[1, features] = SUM_Term[features];
                    MI_R[2, features] = (features+1);
                }//for (int k = 1; k <= Class_count; k++)
            }
        }

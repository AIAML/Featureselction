    class RDC
    {
        private byte[,] Data;
        private byte[,] Class;
        private byte Type_Dataset;
        private byte[] TCMAX;
        private int[,,] Document_frequencies_terms;
        private byte Class_count ;
        private float[,] RDC_info;
        private double[,] AUCt;
        private int[] SUM_Term;
        string ds = "";
        public void GetData(byte[,] Data_t, byte[,] Class_t, byte Type_Dataset_t)
        {
            this.Data = Data_t;
            this.Class = Class_t;
            this.Type_Dataset = Type_Dataset_t;
            this.TCMAX = new byte[Data_t.GetLength(1)];
            this.SUM_Term = new int[Data.GetLength(1)];
            
            if (Type_Dataset == 0 )
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
        public void Calculate_RDC(string saveResult_url)
        {
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
            MLAppClass matlab = new MLAppClass();
            byte max = 0;
            byte max_all = 0;
            for(int features=0; features < Data.GetLength(1); features++)
            {
                for(int row=0; row < Data.GetLength(0); row++)
                {
                    if(Data[row, features] >max)
                    {
                        max = Data[row, features];
                    }
                }
                TCMAX[features] = max;
                if(max > max_all)
                {
                    max_all = max;
                }
                max = 0;
            }
                Document_frequencies_terms = new int[max_all, Data.GetLength(1) , Class_count];


            this.RDC_info = new float[max_all, Data.GetLength(1)];
            this.AUCt = new double[3,Data.GetLength(1)];

            int count_accourance_perclass = 0;
            int Sum_count_accourance_per_feature = 0;
            double AVG_count_accourance_per_feature = 0;
            double Sum_Variance = 0;
            double standard_devision_Variance = 0;
            int min_count_per_term ;
            int max_count_per_term;
            double difference_TC;
            
            for (int i = 1; i <= max_all; i++)
            {
                
                for (int features = 0; features < Data.GetLength(1); features++)
                {
                    Sum_count_accourance_per_feature = 0;
                    for (int k = 1; k <= Class_count; k++)
                    {
                        count_accourance_perclass = 0;
                        for (int row = 0; row < Data.GetLength(0); row++)
                        {
                            if (Data[row, features] == i && Class[row,0] == k)
                            {
                                count_accourance_perclass++;
                                Document_frequencies_terms[i-1, features, k-1] = count_accourance_perclass;
                            }
                        }
                        Sum_count_accourance_per_feature = count_accourance_perclass;
                    }
                    AVG_count_accourance_per_feature = (double)((double)Sum_count_accourance_per_feature / (double)Class_count);
                    Sum_Variance = 0;
                    min_count_per_term = 254;
                    max_count_per_term = 0;
                    if ((i == 1 && features == 3) || (i == 2 && features == 3))
                    {
                        int JJ = 1;
                        JJ++;
                    }
                    for (int k = 1; k <= Class_count; k++)
                    {
                        if (Document_frequencies_terms[i - 1, features, k - 1] > max_count_per_term)
                            max_count_per_term = Document_frequencies_terms[i - 1, features, k - 1];
                        else if (Document_frequencies_terms[i - 1, features, k - 1] < min_count_per_term)
                            min_count_per_term = Document_frequencies_terms[i - 1, features, k - 1];
                        Sum_Variance = Sum_Variance + Math.Pow(Document_frequencies_terms[i - 1, features, k - 1]-  AVG_count_accourance_per_feature ,2);
                    }//End for (int k = 1; k <= Class_count; k++)


                    standard_devision_Variance = Math.Sqrt((double)((double)Sum_Variance / (double)Class_count));
                    


                    difference_TC = max_count_per_term - min_count_per_term;


                    if(min_count_per_term == 0)
                        RDC_info[i - 1, features] = (float)((float)difference_TC / (float) (0.1*i));
                    else
                        RDC_info[i - 1, features] = (float)((float)difference_TC / (float)(min_count_per_term * i));
                }//End of for (int features = 0; features < Data.GetLength(1); features++)
            }//End of for (int i = 1; i <= max_all; i++)
            float AUCt_value =0;
            matlab.PutWorkspaceData("RDC_info", "base", RDC_info);
            for (int features = 0; features < Data.GetLength(1); features++)
            {
                AUCt_value = 0;
                for (int i = 1; i <= max_all; i++)
                {

                    if( i == max_all)
                    {
                        AUCt_value = (float)((float)(RDC_info[i-1, features] + 0) / (float)2) + AUCt_value;
                    }
                    else
                    {
                        AUCt_value = (float)((float)(RDC_info[i, features] + RDC_info[i - 1, features]) / (float)2) + AUCt_value;
                    }
                    
                }
                AUCt[0,features] = AUCt_value;
                AUCt[1, features] = SUM_Term[features];
                AUCt[2, features] = (features+1);
            }

            matlab.PutWorkspaceData("RDC", "base", AUCt);
            matlab.Execute(@"RDC=(RDC')");
            matlab.Execute(@"RDC=sortrows(RDC,-1)");
            matlab.Execute(@"RDC=(RDC')");
            matlab.Execute(@"save('" + saveResult_url + "RDC_" + ds + ".mat');");

        }
    }

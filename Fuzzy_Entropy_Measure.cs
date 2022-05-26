// Calculate Fuzzy Entropy metric for each feature (Word)
// sort features based on their Fuzzy entropy value
// Remove some percentage of features
 
class FuzzyEntropy
    {
        MLAppClass matlab = new MLAppClass();
        //Intialize Varaibles
        private byte[,] Data;
        private byte[,] Class;
        byte[] Data_max;
        double[,] Sim ;
        private byte Type_Dataset;

        private double[] Sum_Sample_feature;
        private double[,] V;
        double[] V_max;
        double similarity;
        double sum_entropy;
        double[] SUM_E;
        double[,] Entropy;
        private int[] SUM_Term;
        int rows;
        int columns;
        public void GetData(byte[,] Data_t , byte[,] Class_t , byte Type_Dataset_t)
        {
            this.Data = Data_t;
            this.Class = Class_t;
            this.Type_Dataset = Type_Dataset_t;
            this.SUM_Term = new int[Data.GetLength(1)];
        }
        public void Fuzzy_entropy_calculate(string saveResult_url)
        {
            int number_class = 1,i,j;
            int[] count_class;
            int sample_count = Data.GetLength(0);
            int feature_count = Data.GetLength(1);
            string ds = "";
            if (this.Type_Dataset == 0)
            {
                //dataset id=0 then reuters21758
                number_class = 65;
                ds = "Reuters";
            }
            else if (this.Type_Dataset == 1)
            {
                //dataset id=0 then reuters21758
                number_class = 20;
                ds = "20newsgroup";
            }
            MLAppClass matlab = new MLAppClass();
            for (int ii = 0; ii < Data.GetLength(1); ii++)
            {
                for (int jj = 0; jj < Data.GetLength(0); jj++)
                {
                    if (Data[jj, ii] > 0)
                    {
                        SUM_Term[ii] = SUM_Term[ii] + Data[jj, ii];
                    }
                }
            }


            count_class = new int[number_class];
            Sum_Sample_feature = new double[number_class];
            V = new double[number_class,feature_count];
      //count number of Doucuments per each class
            for (i = 0; i < number_class; i++)
            {
                for (j = 0; j < Data.GetLength(0); j++)
                {
                    if ((Class[j,0] - 1) == i)
                        count_class[i]++;
                }
                
            }//end of for (i = 0; i < number_class; i++)
      //Calculate Data to learn
            for (i = 0; i < feature_count ; i++)
            {
                for (j = 0; j < sample_count ; j++)
                {
                    Sum_Sample_feature[Class[j, 0] - 1] = Sum_Sample_feature[Class[j, 0] - 1] + Math.Pow((Data[j,i]),2);
                }
                for (int k = 0; k < number_class; k++)
                {
                    V[k,i] = Math.Sqrt((double)Sum_Sample_feature[k] / count_class[k]);
                    Sum_Sample_feature[k] = 0;
                }
            }
     //find max o min for V
            Data_max = new byte[feature_count];
            V_max = new double[number_class];

            for (i = 0; i < number_class; i++)
            {
                for (j = 0; j < feature_count; j++)
                {
                    if (V[i, j] > V_max[i])
                        V_max[i] = V[i, j];
                }
            }
    //find Max o Min for Data
            for (i = 0; i < feature_count; i++)
            {
                for (j = 0; j < sample_count; j++)
                {
                    if (Data[j,i] > Data_max[i])
                        Data_max[i] = Data[j, i];
                }
            }
   //calculate similarity
            Entropy = new double[3,feature_count];
            SUM_E = new double[feature_count];
            Sim = new double[sample_count, number_class];
            double d, max_d, V_D, max_V_D;
            double[,] s_c = new double[sample_count, number_class];

            sum_entropy = 0;

            for (i = 0; i < feature_count; i++)
            {
                for (j = 0; j < sample_count; j++)
                {
                    for (int k = 0; k < number_class; k++)
                    {
                        similarity = 1 - Math.Abs(((double)Data[j, i] / Data_max[i]) - ((double)V[k, i] / V_max[k]));
                        similarity = Math.Abs(similarity);
                        s_c[j, k] = similarity;

                        if (similarity > 0 && similarity < 1)
                            sum_entropy = sum_entropy + (similarity * (Math.Log(similarity)) + (1 - similarity)) * (Math.Log((1 - similarity)));
                        else if (similarity == 0)
                            sum_entropy = sum_entropy;
                        else if (similarity == 1)
                            sum_entropy = sum_entropy - 1;
                    }                    
                }
                Entropy[0,i] = (-1 * sum_entropy);
                if (Entropy[0, i] == 0)
                    Entropy[0, i] = 9999999;
                Entropy[1, i] = SUM_Term[i];
                Entropy[2, i] = (i+1);
                sum_entropy = 0;   
            }
            matlab.PutWorkspaceData("Entropy", "base", Entropy);
            matlab.Execute(@"Entropy=(Entropy')");
            matlab.Execute(@"Entropy=sortrows(Entropy,1)");
            matlab.Execute(@"Entropy=(Entropy')");
            matlab.Execute(@"save('" + saveResult_url + "Entropy_" + ds +".mat');");
            //calculate fuzzy entropy
        }

    }

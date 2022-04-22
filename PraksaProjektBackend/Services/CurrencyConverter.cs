namespace PraksaProjektBackend.Services
{
    public class CurrencyConverter
    {
        public static double USD_To_BAM(double usd)
        {
            double bam = usd * 1.81;
            return bam;
        }
        public static double BAM_To_USD(double bam)
        {
            double usd = bam *0.55;
            return usd;
        }
        public static double BAM_To_HRK(double bam)
        {
            double hrk = bam * 3.86;
            return hrk;
        }
        public static double HRK_To_BAM(double hrk)
        {
            double bam = hrk * 0.26;
            return bam;
        }
        public static double HRK_To_USD(double hrk)
        {
            double usd = hrk * 0.14;
            return usd;
        }
        public static double USD_To_HRK(double usd)
        {
            double hrk = usd * 6.99;
            return hrk;
        }
        public static double EUR_To_HRK(double eur)
        {
            double hrk = eur * 7.56;
            return hrk;
        }
        public static double HRK_To_EUR(double hrk)
        {
            double eur = hrk * 0.13;
            return eur;
        }
        public static double BAM_To_EUR(double bam)
        {
            double eur = bam * 0.51;
            return eur;
        }
        public static double EUR_To_BAM(double eur)
        {
            double bam = eur * 1.96;
            return bam;
        }
    }
}

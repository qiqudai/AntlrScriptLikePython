using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntacticSugar
{
    public class PyRandom
    {
        private Random _random;
        private int _seed;

        public PyRandom(int seed)
        {
            _random = new Random(seed);
            _seed = seed;
        }

        public PyRandom() : this(Environment.TickCount)
        {
        }

        // seed()
        public void seed(int seed)
        {
            _random = new Random(seed);
            _seed = seed;
        }

        // getstate()
        public string getstate()
        {
            return _random.ToString();
        }

        // setstate()
        public void setstate(string state)
        {
            // This is a simplified version as `state` can't be directly restored in .NET
            // A more complex implementation might involve saving and restoring the exact state of the Random object.
            throw new NotImplementedException("setstate() is not fully implemented.");
        }

        // getrandbits()
        public int getrandbits(int k)
        {
            int bits = 0;
            for (int i = 0; i < k; i++)
            {
                bits = (bits << 1) | (_random.Next(2) & 1);
            }

            return bits;
        }

        // randrange()
        public int randrange(int start, int stop, int step = 1)
        {
            if (step == 1)
                return _random.Next(start, stop);
            else
                throw new NotImplementedException("Only step = 1 is supported.");
        }

        // randint()
        public int randint(int a, int b)
        {
            return _random.Next(a, b + 1);
        }

        // choice()
        public pyint choice(list seq)
        {
            if (seq.Count == 0) throw new InvalidOperationException("Cannot choose from an empty sequence.");
            return seq[_random.Next(seq.Count)];
        }

        // choices()
        public list choices(list population, list weights = null, int k = 1)
        {
            if (population == null || population.Count == 0)
                throw new ArgumentException("Population cannot be null or empty");

            if (weights != null && population.Count != weights.Count)
                throw new ArgumentException("Population and weights must have the same length");

            list result = new();

            if (weights == null)
            {
                // Uniform distribution (no weights provided)
                for (int i = 0; i < k; i++)
                {
                    int index = _random.Next(population.Count);
                    result.append(population[index]);
                }
            }
            else
            {
                // Weighted distribution
                double totalWeight = weights.Sum(w => w.get<double>());
                var cumulativeWeights = new List<double>();
                double cumulativeSum = 0;

                foreach (var weight in weights)
                {
                    cumulativeSum += weight.get<double>();
                    cumulativeWeights.Add(cumulativeSum);
                }

                for (int i = 0; i < k; i++)
                {
                    double randValue = _random.NextDouble() * totalWeight;

                    // Binary search for the selected weight
                    int index = cumulativeWeights.BinarySearch(randValue);
                    if (index < 0) index = ~index;

                    result.append(population[index]);
                }
            }

            return result;
        }

        // shuffle()
        public void shuffle(list x)
        {
            int n = x.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                pyint value = x[k];
                x[k] = x[n];
                x[n] = value;
            }
        }

        // sample()
        public list sample(list population, int k)
        {
            if (k > population.Count)
                throw new ArgumentException("Sample size cannot be larger than the population size.");
            return choices(population, k: k);
        }

        // random()
        public double random()
        {
            return _random.NextDouble();
        }

        // uniform()
        public double uniform(double a, double b)
        {
            if (b < a)
                return _random.NextDouble() * (a - b) + b;
            else
                return _random.NextDouble() * (b - a) + a;
        }

        // triangular()
        public double triangular(double low, double high, double mode)
        {
            double u = _random.NextDouble();
            double c = (mode - low) / (high - low);
            if (u < c)
                return low + Math.Sqrt(u * (high - low) * (mode - low));
            else
                return high - Math.Sqrt((1 - u) * (high - low) * (high - mode));
        }

        // betavariate()
        public double betavariate(double alpha, double beta)
        {
            return new Random().NextDouble(); // Placeholder; implement Beta distribution if needed.
        }

        // expovariate()
        public double expovariate(double lambd)
        {
            return -Math.Log(1.0 - _random.NextDouble()) / lambd;
        }

        // gammavariate()
        public double gammavariate(double alpha, double beta)
        {
            // Placeholder; you could implement the Gamma distribution using a method such as Marsaglia & Tsang's method.
            return _random.NextDouble(); // Simplified version
        }

        // gauss()
        public double gauss(double mu, double sigma)
        {
            double u1 = _random.NextDouble();
            double u2 = _random.NextDouble();
            double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mu + z0 * sigma;
        }

        // lognormvariate()
        public double lognormvariate(double mu, double sigma)
        {
            return Math.Exp(gauss(mu, sigma));
        }

        // normalvariate()
        public double normalvariate(double mu, double sigma)
        {
            return gauss(mu, sigma);
        }

        // vonmisesvariate()
        public double vonmisesvariate(double mu, double kappa)
        {
            // Placeholder for Von Mises distribution.
            return mu + _random.NextDouble(); // Simplified version
        }

        // paretovariate()
        public double paretovariate(double alpha)
        {
            return Math.Pow(1 - _random.NextDouble(), -1.0 / alpha);
        }

        // weibullvariate()
        public double weibullvariate(double alpha, double beta)
        {
            return Math.Pow(-Math.Log(1 - _random.NextDouble()), 1.0 / alpha) * beta;
        }

        // 测试代码
        public static void TEST()
        {
            var pyRandom = new PyRandom(42);

            // Test random methods
            Console.WriteLine(pyRandom.randint(1, 10));
            Console.WriteLine(pyRandom.choice(new list
                { new pyint(1), new pyint(2), new pyint(3), new pyint(4) }));
            Console.WriteLine(pyRandom.random());
            Console.WriteLine(pyRandom.uniform(1.0, 5.0));
        }
    }
}
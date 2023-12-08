using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _21f_kupac_cs
{
	class Program
	{
		class Kupac
		{
			public List<int> t;
			Func<int, int, bool> comparator;
			public Kupac(Func<int, int, bool> comparator)
			{
				this.comparator = comparator;
				this.t = new List<int>();
				t.Add(0);
			}

			public int Count => t.Count - 1;
			public int Peek() => t[1];
			public int Pop()
			{
				int result = Peek();
				Csere(1, Count);
				t.RemoveAt(t.Count - 1);
				Sullyeszt(1);
				return result;
			}
			private void Csere(int i, int j) => (t[i], t[j]) = (t[j], t[i]);

			public void Push(int e)
			{
				t.Add(e);
				Fellebegtet(Count);
			}

			public void Repair(int e) => Megigazit(Keres(1, e));
			private int Szulo(int n) => n == 1 ? n : n / 2;
			private int Kisebbik_gyerek(int n)
			{
				int egyik = n * 2;
				int masik = egyik + 1;
				if (Count < egyik)
					return -1;
				if (Count < masik)
					return egyik;
				return Kisebbik(egyik, masik);
			}
			private int Kisebbik(int a, int b) => comparator(t[a], t[b]) ? a : b;
			private void Sullyeszt(int n)
			{
				int kgy = Kisebbik_gyerek(n);
				if (kgy == -1)
					return;
				if (comparator(t[kgy], t[n]))
				{
					Csere(kgy, n);
					Sullyeszt(kgy);
				}
			}
			private void Fellebegtet(int n)
			{
				while (comparator(t[n], t[Szulo(n)])) // t[n] < t[szulo(n)]
				{
					Csere(n, Szulo(n));
					n = Szulo(n);
				}
			}

			int Keres(int hol, int elem)
			{
				if (t[hol] == elem)
					return hol;
				int egyik = 2 * hol;
				if (Count < egyik)
					return -1;
				int az_eredmeny = Keres(egyik, elem);
				if (az_eredmeny != -1)
					return az_eredmeny;
				int masik = 2 * hol + 1;
				if (Count < masik)
					return -1;
				az_eredmeny = Keres(masik, elem);
				if (az_eredmeny != -1)
					return az_eredmeny;
				return -1;
			}
			void Megigazit(int n)
			{
				Sullyeszt(n);
				Fellebegtet(n);
			}

		}

		static void Main(string[] args)
		{


			Kupac k = new Kupac((int a, int b) => a > b);

			for (int i = -10; i < 30; i += 2)
			{
				k.Push(i);
			}
			for (int i = -21; i < 11; i += 2)
			{
				k.Push(i);
			}

			while (k.Count!=0)
			{
				Console.WriteLine(k.Pop());
			}

		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace _21f_kupac_cs
{
	class Program
	{
		class Kupac<T>
		{
			// --- VÁLTOZÓK --- 

			public List<T> t;
			Func<T, T, int> comparator;

			// --- KONSTRUKTOROK ---

			/// <summary>
			/// Kupac adatszerkezet, mindig a megadott comparator szerint legkisebb elemet adja ki. 
			/// </summary>
			/// <param name="comparator">háromértékű rendezés</param>
			public Kupac(Func<T, T, int> comparator)
			{
				this.comparator = comparator;
				this.t = new List<T>();
			}
			
			public Kupac(Func<T, T, int> comparator, IEnumerable<T> kezdoertekek) : this(comparator)
			{
				foreach (T elem in kezdoertekek)
					Push(elem);
			}


			// --- PUBLIKUS METÓDUSOK ---

			public int Count { get => t.Count; }
			public bool Empty() => t.Count == 0;
			public T Peek() => t[Elso];
			public void Push(T e)
			{
				t.Add(e);
				Fellebegtet(Utolso);
			}
			public T Pop()
			{
				T result = Peek();
				Csere(Elso, Utolso);
				t.RemoveAt(Utolso);
				Sullyeszt(Elso);
				return result;
			}
			public void Repair(T e)
			{
				List<int> elem_helyei = Tombkeres(e);
				foreach (int i in elem_helyei)
					Megigazit(i);
			}
			public string ToGraphviz() => $"digraph{{\n {ToGraphviz(0)}\n}}\n";
			public void Diagnosztika() => Console.WriteLine(this.ToGraphviz());
			public void Keres_teszt()
			{
				Console.WriteLine("KERESÉS TESZT: MEGVIZSGÁLJUK, HOGY MINDEN ELEMRE UGYANAZT ADJA-E A KÉT KERESÉS!");
				foreach (T elem in t)
				{
					List<int> elem_helyei = Tombkeres(elem);
					int i = Keres(elem);
					Console.WriteLine($"Az {elem} helye tömb szerint {string.Join(", ", elem_helyei)} és fa szerint {i} --> {(elem_helyei.Contains(i)?"SZUPER":"nemjó....")}");
				}
			}

			// --- TÖMBMETÓDUSOK ---

			int Elso { get => 0; }
			int Utolso { get => t.Count - 1; }
			void Csere(int i, int j) => (t[i], t[j]) = (t[j], t[i]);
			List<int> Tombkeres(T elem)
			{
				List<int> result = new List<int>();
				for (int i = 0; i < t.Count; i++)
				{
					if (t[i].Equals(elem))
						result.Add(i);
				}
				return result;
			}

			// --- TELJES BINÁRIS FA-METÓDUSOK ---

			int Szulo_indexe(int n) => n == 0 ? n : ((n + 1) / 2 - 1);
			List<int> Gyerekek_indexei(int n)
			{
				int kisebbik_indexe = 2 * n + 1;
				if (t.Count <= kisebbik_indexe)
					return new List<int>();

				int nagyobbik_indexe = kisebbik_indexe + 1;
				if (t.Count <= nagyobbik_indexe)
					return new List<int> { kisebbik_indexe };

				return new List<int> { kisebbik_indexe, nagyobbik_indexe };
			}
			int Kisebbik_gyerek_indexe(int n)
			{
				List<int> gyerekek = Gyerekek_indexei(n);
				switch (gyerekek.Count)
				{
					case 0:
						return -1;
					case 1:
						return gyerekek[0];
					case 2:
						return Kisebbik_ertek_indexe(gyerekek[0], gyerekek[1]);
				}
				throw new Exception("nem szabadna 2-nél több gyerek legyen!");
			}
			int Kisebbik_ertek_indexe(int a, int b) => comparator(t[a], t[b]) == -1 ? a : b;
			void Sullyeszt(int n)
			{
				int kgyi = Kisebbik_gyerek_indexe(n);
				if (kgyi == -1)
					return;
				if (comparator(t[kgyi], t[n]) == -1)
				{
					Csere(kgyi, n);
					Sullyeszt(kgyi);
				}
			}
			void Fellebegtet(int n)
			{
				while (comparator(t[n], t[Szulo_indexe(n)]) == -1)
				{
					Csere(n, Szulo_indexe(n));
					n = Szulo_indexe(n);
				}
			}
			void Megigazit(int n)
			{
				Sullyeszt(n);
				Fellebegtet(n);
			}
			/// <summary>
			/// nagyon hatékonyan megkeresi egy elem helyét a fában, feltéve, hogy a kupacban nincsenek rendezési hibák!
			/// </summary>
			/// <param name="elem"></param>
			/// <returns></returns>
			int Keres(T elem) => Keres(0, elem);
			int Keres(int hol, T elem)
			{
				//Console.WriteLine($"t[{hol}] = {t[hol]} pontban keresem {elem}-et, összehasonlítás eredménye: {comparator(elem, t[hol])}");
				if (elem.Equals(t[hol]))
					return hol;
				switch (comparator(elem, t[hol]))
				{
					case -1:
						//Console.WriteLine($"nincs itt és nem is lesz itt: t[{hol}] = {t[hol]}");
						return -1;
					case 0:
						// Console.WriteLine($"meg is van itt: t[{hol}] = {t[hol]}");
						// return hol; // nem ugyanaz a nagyság szerinti megkülönböztethetetlenség és az azonosság!
					default:
						//Console.WriteLine($"nincs itt, de tovább kell keresni itt: t[{hol}] = {t[hol]}");
						List<int> gyi = Gyerekek_indexei(hol);
						switch (gyi.Count)
						{
							case 0:
								//Console.WriteLine($"Nincs gyereke sem már: t[{hol}] = {t[hol]}");
								return -1;
							case 1:
								//Console.WriteLine($"Egy gyerek van már csak neki: t[{hol}] = {t[hol]}, itt keresünk tovább");
								return Keres(gyi[0], elem);
							default:
								//Console.WriteLine($"Két gyerek van: t[{hol}] = {t[hol]}, először az elsőnél keresünk, és ha az nem válik be, jön a másik");
								int result = Keres(gyi[0], elem);
								return result != -1 ? result : Keres(gyi[1], elem);
						}
				}
			}
			string ToGraphvizNode(int i, T e) => $"    {i} [label=<{e}<SUB>{i}</SUB>>];\n";
			string ToGraphviz(int n)
			{
				List<int> gyi = Gyerekek_indexei(n);
				switch (gyi.Count)
				{
					case 0:
						return ToGraphvizNode(n, t[n]);
					case 1:
						return $"{ToGraphvizNode(n, t[n])}    {n} -> {gyi[0]};\n" + ToGraphviz(gyi[0]);
					default:
						return $"{ToGraphvizNode(n, t[n])}    {n} -> {gyi[0]};\n    {n} -> {gyi[1]};\n" + ToGraphviz(gyi[0]) + ToGraphviz(gyi[1]);
				}
			}
		}


		static void Main(string[] args)
		{
			/// Példakupac 
			/** / 
			#region probakupac
			Kupac<int> k = new Kupac<int>((int a, int b) => a.CompareTo(b));
			for (int i = -10; i < 30; i += 2)
				k.Push(i);
			for (int i = -31; i < 21; i += 5)
				k.Push(i);
			k.Diagnosztika();
			#endregion
			/**/

			// k.Keres_teszt();

			// Dijkstra(m, 0);
			/**/



			(List<int[]> m, int honnan, int hova) = Fizetos_utak_Beolvas();

			/**/
			for (int i = 0; i < m.Count; i++)
			{
				for (int j = 0; j < m[0].Length; j++)
				{
					Console.Write(m[i][j]);
					Console.Write("\t");
				}
				Console.WriteLine();
			}
			/**/

			(int[] tav, int[] honnanvektor) = Dijkstra(m, honnan);

			Console.WriteLine($"tav = [{string.Join(", ", tav)}]");
			Console.WriteLine($"honnan vektor = [{string.Join(", ", honnanvektor)}]");

			Console.ReadLine();

		}


		static (List<int[]>, int, int) Fizetos_utak_Beolvas()
		{
			string[] st = Console.ReadLine().Split(' ');
			int N = int.Parse(st[0]);
			int E = int.Parse(st[1]);

			List<int[]> m = new List<int[]>();


			for (int i = 0; i < N; i++)
			{
				int[] sor = new int[N];
				for (int j = 0; j < N; j++)
				{
					if (i == j)
						sor[j] = 0;
					else
						sor[j] = -1;
				}
				m.Add(sor);
			}

			for (int i = 0; i < E; i++)
			{
				st = Console.ReadLine().Split(' ');
				m[int.Parse(st[0]) - 1][int.Parse(st[1]) - 1] = st[2] == "F" ? 1 : 0;
				m[int.Parse(st[1]) - 1][int.Parse(st[0]) - 1] = st[2] == "F" ? 1 : 0;
			}

			st = Console.ReadLine().Split(' ');
			int honnan = int.Parse(st[0]) - 1;
			int hova = int.Parse(st[1]) - 1;

			return (m, honnan, hova);
		}

		static List<int> Szomszédai(List<int[]> m, int csucs)
		{
			List<int> szomszedok = new List<int>();
			for (int i = 0; i < m.Count; i++)
				if (m[csucs][i] >= 0)
					szomszedok.Add(i);
			return szomszedok;
		}

		static int Plafonos_összeadás(int a, int b)
		{
			int c = a + b;
			if (0 < a && 0 < b && c < 0)
				return int.MaxValue;
			return c;
		}

		private static (int[], int[]) Dijkstra(List<int[]> m, int v)
		{
			int[] tav = new int[m.Count];
			for (int i = 0; i < m.Count; i++)
			{
				tav[i] = int.MaxValue;
			}
			tav[v] = 0;

			Kupac<int> tennivalok = new Kupac<int>((int x, int y) => tav[x].CompareTo(tav[y]));

			int[] honnan = new int[m.Count];
			for (int i = 0; i < m.Count; i++)
			{
				honnan[i] = -2;
			}

			honnan[v] = -1;

			for (int i = 0; i < m.Count; i++)
			{
				tennivalok.Push(i);
			}

			while (tennivalok.Count != 0)
			{
				int tennivalo = tennivalok.Pop();
				foreach (var szomszéd in Szomszédai(m, tennivalo))
				{
					int új_jelölt = Plafonos_összeadás(tav[tennivalo], m[tennivalo][szomszéd]);
					if (új_jelölt < tav[szomszéd])
					{
						tav[szomszéd] = új_jelölt;
						tennivalok.Repair(szomszéd);
						honnan[szomszéd] = tennivalo;
					}
				}
			}

			return (tav, honnan);

		}
	}
}/*

7 11
1 2 F
1 5 I
2 5 I
2 3 I
5 3 F
5 6 F
3 6 I
3 4 F
6 4 F
6 7 I
4 7 F
1 4

*/

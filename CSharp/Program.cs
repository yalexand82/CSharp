#define WE_BE_DEBUGIN

using System;
using System.Collections.Generic;
using System.Linq;
//using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics; //Stopwatch
using System.IO;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Reflection; //Assembly
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using BFF = System.Reflection.BindingFlags;
using System.Windows.Forms.VisualStyles;
using System.Globalization;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;

namespace CSharp
{
    // *** # *** X
    public class X
    {
        public class XMainClass
        {
            public static void XMain()
            {
            }
        }
    }

    #region CONTAINERS

    #region Arrays

    class ImplicitilyTypedArrays
	{
		class Base { }
		class Derived1 : Base { }
		class Derived2 : Base { }
		class Cow
		{
			public static implicit operator Derived1(Cow cow) { return null; }
		}
		void ImplicitilyTypedArrays_Example()
		{
			var myInstance = new { FirstName = "Yianni", LastName = "Alexander" };
			var yourInstance = new { FirstName = "Ricky", LastName = "Bobby" };

			var myInstanceArray = new[] { myInstance, yourInstance };

			//var implicitArray = new[] { new Derived1(), new Derived2() }; //No best type found for implicitly-typed array
			var myArray = new[] { new Derived1(), new Base(), new Derived2(), new Cow() };
			Console.WriteLine(myArray.GetType());
		}
	}

	class ArrayValueTypesVsReference
	{
		class MeClass { public int MeField; } // Reference Types
		struct MeStruct { public int MeField; } // Value Types

		static void ReferenceTypes()
		{
			MeClass[] meClasses = new MeClass[3];
			meClasses[1] = new MeClass();
			meClasses[1].MeField = 5;
		}

		static void ValueTypes()
		{
			MeStruct[] meStructs = new MeStruct[3];
			//meStructs[1] = new MeStruct(); // this does not cause a null reference exception
			meStructs[1].MeField = 5;  // MeField is entire instance of MeStruct struct
		}
	}

	static class ArrayForEach
	{
		//static void ForEach(this int[] ints, Action<int> action) // specific version
		static void ForEach<T>(this T[] array, Action<T> action) // generic version
		{
			foreach (T i in array)
				action(i);
		}

		static void ArrayForEach_Example()
		{
			int[] ints = new int[] { 5, 2, 8, 9, 1, 0, 4 };

			Array.ForEach(ints, Console.WriteLine);  // static

			ints.ForEach(Console.WriteLine);
			ints.ForEach(i => Console.Write(i + " "));
		}
	}

	static class ArrayStaticMethods
	{
		/*					LINQ counterparts
		 * Sort()			OrderBy()
		 * ForEach()
		 * IndexOf()		First()
		 * Clear()
		 * Copy()
		 * CreateInstance()	new int[]
		 * Exists()			Any()
		 * Find()			First()
		 * FindLast()		Last()
		 * FindAll()		Where()
		 * FindIndex()
		 * FindLastIndex()
		 * Resize()
		 * Reverse()		Reverse()
		 */
		static void ArrayStaticMethods_Example()
		{
			int[] ints = new int[] { 5, 2, 8, 9, 1, 0, 4 };
			Array.Sort(ints);
			IEnumerable<int> sorted = ints.OrderBy(i => i);
		}
	}

	static class ArrayListVsList
	{
		static void ArrayListVsList_Example()
		{
			// Dynamic Containers
			ArrayList myPartyAges = new ArrayList();
			myPartyAges.Add(25);
			myPartyAges.Add(34);
			myPartyAges.Add("Bill");
			myPartyAges.Add(32);
			myPartyAges.Add(99);

			List<int> myPartyAges2 = new List<int>();
			myPartyAges2.Add(25);
			myPartyAges2.Add(34);
			//myPartyAges2.Add("Bill"); // doesn't work
			myPartyAges2.Add(32);
			myPartyAges2.Add(99);
		}
	}

	#endregion Arrays

	class List_IEnumerator_IEnumerable
	{
		// Our definition of .NET's List Container: List<T>
		class MeList<T> : IEnumerable<T>
		{
			T[] items;
			public MeList(int capacity = 5)
			{
				items = new T[capacity];
			}
			public void Add(T item)
			{
				EnsureCapacity();
				items[Count++] = item;
			}
			public void AddRange(IEnumerable<T> newItems)
			{
				EnsureCapacity(Count + newItems.Count());
				foreach (T newItem in newItems)
					Add(newItem);
			}
			// allows class to be used with foreach
			public IEnumerator<T> GetEnumerator()
			{
				//return new MeEnumerator(this);
				for (int i = 0; i < Count; i++)
					yield return items[i];
			}
			
			class MeEnumerator : IEnumerator<T>
			{
				int index = -1;
				MeList<T> theList;

				public MeEnumerator(MeList<T> theList)
				{
					this.theList = theList;
				}

				public bool MoveNext()
				{
					return ++index < theList.Count;
				}

				public T Current
				{
					get
					{
						if (index < 0 || index >= theList.Count)
							return default(T);
						return theList.items[index];
					}
				}

				public void Dispose() { }

				object IEnumerator.Current { get { return Current; } }

				public void Reset() { index = -1; }
			}
			
			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
			public T this[int index] // Indexer, allows us to reference MeList using []
			{
				get
				{
					CheckBoundaries(index);
					return items[index];
				}
				set
				{
					CheckBoundaries(index);
					items[index] = value;  // value is implicit argument
				}
			}
			void CheckBoundaries(int index)
			{
				if (index >= Count || index < 0)
					throw new IndexOutOfRangeException();
			}
			public int Capacity
			{
				get { return items.Length; }
			}
			public int Count { get; set; }
			public void Clear()
			{
				Count = 0;
				// only need to do this (to pick up garbage collection) if T is NOT a Value Type
				for (int i = 0; i < Count; i++)
				{
					items[i] = default(T);
				}
			}
			public void TrimExcess()
			{
				T[] newArray = new T[Count];
				Array.Copy(items, newArray, Count);
				items = newArray;
			}
			public void Insert(int index, T item)
			{
				EnsureCapacity();
				// shuffle everyone down the existing array
				Array.Copy(items, index, items, index + 1, Count - index);
				items[index] = item;
				Count++;
			}
			public void InsertRange(int index, IEnumerable<T> newItems)
			{
				T[] newItemsArray = newItems.ToArray();

				if (Count + newItemsArray.Length > Capacity)
				{
					T[] newUnderlyingArray = new T[Count + newItemsArray.Length];
					Array.Copy(items, newUnderlyingArray, index);
					Array.Copy(items, index, newUnderlyingArray, index + newItemsArray.Length, Count - index);
					items = newUnderlyingArray;
				}
				else
				{
					// shuffle everyone down the existing array
					Array.Copy(items, index, items, index + newItemsArray.Length, Count - index);
				}
				Array.Copy(newItemsArray, 0, items, index, newItemsArray.Length);
				Count += newItemsArray.Length;
			}
			public MeList<T> GetRange(int index, int amount)
			{
				MeList<T> ret = new MeList<T>(amount);
				Array.Copy(items, index, ret.items, 0, amount);
				ret.Count = amount;
				return ret;
			}
			public void RemoveAt(int index)
			{
				Array.Copy(items, index + 1, items, index, Count - (index + 1));
				Count--;
			}
			public void RemoveRange(int index, int amount)
			{
				Array.Copy(items, index + amount, items, index, Count - (amount + 1));
				Count -= amount;
			}
			public bool Remove(T item)
			{
				for (int i = 0; i < Count; i++)
				{
					if (items[i].Equals(item))
					{
						RemoveAt(i);
						return true;
					}
				}
				return false;
			}
			public void RemoveAll(Predicate<T> predicate)
			{
				for (int i = 0; i < Count; i++)
				{
					if (predicate(items[i]))
					{
						RemoveAt(i);
						i--;
					}
				}
			}

			public bool Contains(T item) { return true;	} // NOT DEFINED
			public bool Exists(Predicate<T> predicate) { return true; } // NOT DEFINED
			public T Find(Predicate<T> predicate) { return default(T); } // NOT DEFINED
			public MeList<T> FindAll(Predicate<T> predicate) { return null; } // NOT DEFINED
			public int FindIndex(Predicate<T> predicate) { return -1; } // NOT DEFINED
			public T FindLast(Predicate<T> predicate) { return default(T); } // NOT DEFINED
			public int FindLastIndex(Predicate<T> predicate) { return -1; } // NOT DEFINED
			public int IndexOf(T item) { return -1; } // NOT DEFINED
			public int LastIndexOf(T item) { return -1; } // NOT DEFINED

			public bool TrueForAll(Predicate<T> condition)
			{
				for (int i = 0; i < Count; i++)
					if (!condition(items[i]))
						return false;
				return true;
			}
			public void ForEach(Action<T> action)
			{
				foreach (T item in this)
					action(item);
			}
			public MeList<U> ConvertAll<U>(Converter<T, U> converter)
			{
				MeList<U> ret = new MeList<U>(Count);
				for (int i = 0; i < Count; i++)
					ret.items[i] = converter(items[i]);
				ret.Count = Count;
				return ret;
			}
			public void CopyTo(T[] array)
			{
				Array.Copy(items, array, Count);
			}
			public T[] ToArray()
			{
				T[] ret = new T[Count];
				Array.Copy(items, ret, Count);
				return ret;
			}

			void EnsureCapacity()
			{
				EnsureCapacity(Count + 1);
			}
			void EnsureCapacity(int neededCapacity)
			{
				if (neededCapacity > Capacity)
				{
					int targetSize = items.Length * 2;
					if (targetSize < neededCapacity)
						targetSize = neededCapacity;
					Array.Resize(ref items, targetSize);
				}
			}
		}

		public void List_IEnumerator_IEnumerable_Example()
		{
			List<int> myList = new List<int>();
			myList.Add(25);
			myList.AddRange(Enumerable.Range(0, 1000));
			myList.InsertRange(4, Enumerable.Range(0, 1000));

			//foreach (int i in myList) { Console.WriteLine(i); }
			// translates to:
			IEnumerator<int> rator = myList.GetEnumerator();
			while (rator.MoveNext())
			{
				Console.WriteLine(rator.Current);
			}

			MeList<int> myMeList = new MeList<int>(10) { 11, 22, 33, 44, 55 };
			myMeList.Add(10);
			myMeList.Insert(2, 25);
			myMeList.InsertRange(2, new[] { 55, 65, 75, 79, 55, 68 });

			foreach (int i in myMeList)  // foreach uses paranthetical cast
				Console.WriteLine(i);

			IEnumerator<int> MeRator = myMeList.GetEnumerator();
			try
			{
				while (MeRator.MoveNext())
				{
					Console.WriteLine(MeRator.Current);
				}
			}
			finally { MeRator.Dispose(); }

			int[] aBunchOfItems = Enumerable.Range(0, 100).ToArray();
			IEnumerable<int> anotherBunchOfItems = Enumerable.Range(0, 100); //uses yield

			myMeList.InsertRange(4, aBunchOfItems);
			myMeList.InsertRange(8, anotherBunchOfItems);

			MeList<int> getRangeMeList = myMeList.GetRange(7, 55);

			myMeList.RemoveAt(5);
			myMeList.RemoveRange(2, 3);
			myMeList.Remove(55);
			myMeList.RemoveAll(i => i > 65);

			// Contains, Exists, Find, FindAll, FindIndex, FindLast, FindLastIndex, LastIndexOf, IndexOf

			// List Methods vs LINQ Extension Methods
			bool myListExists = myList.Exists(i => i > 65);
			bool myListAny = myList.Any(i => i > 65);

			IEnumerable<int> myListFindAll = myList.FindAll(i => i > 65);
			IEnumerable<int> myListWhere = myList.Where(i => i > 65);

			bool myListTrueForAll = myList.TrueForAll(i => i > 0);
			bool myMeListTrueForAll = myMeList.TrueForAll(i => i > 0);

			myList.ForEach(Console.WriteLine);
			myList.ForEach(i => Console.Write(i + " "));
			myMeList.ForEach(Console.WriteLine);
			myMeList.ForEach(i => Console.Write(i + " "));

			List<string> myListAsStrings = new List<string>();
			MeList<string> myMeListAsStrings = new MeList<string>();

			myListAsStrings = myList.ConvertAll(i => i.ToString());
			myMeListAsStrings = myMeList.ConvertAll(i => i.ToString());

			int[] myListArray = new int[myList.Count];
			myList.CopyTo(myListArray);
			myListArray = myList.ToArray();

			int[] myMeListArray = new int[myList.Count];
			myMeList.CopyTo(myListArray);
			myMeListArray = myList.ToArray();

			// IComparable
			myList.Sort();

			// BinarySearch, Sort, Reverse
			// AsReadOnly
		}
	}

	public class List_IComparable_IComparer
	{
		public class Babe : IComparable<Babe>
		{
			public string Name { get; set; }
            public int Chemistry { get; set; }
            public int Compatibility { get; set; }
            public int Hotness { get; set; }
			public int Intelligence { get; set; }
			public int Confidence { get; set; }
			public int Athleticism { get; set; }
			public int Cooperative { get; set; }
			public int Loving { get; set; }
            public int Authentic { get; set; }
            public double AggregateScore { get; set; }
            static Random rand = new Random();

			public Babe(string name)
			{
				Name = name;
                Chemistry = rand.Next(6, 10);
                Compatibility = rand.Next(6, 10);
				Hotness = rand.Next(6, 10);
				Intelligence = rand.Next(6, 10);
				Confidence = rand.Next(6, 10);
				Athleticism = rand.Next(6, 10);
                Cooperative = rand.Next(6, 10);
				Loving = rand.Next(6, 10);
                Authentic = rand.Next(6, 10);

            }
			public Babe(string name, int chemistry = 5, int compatibility = 5, int hotness = 5, int intelligence = 5, int confidence = 5, int athleticism = 5, int cooperative = 5, int loving = 5, int authentic = 5)
			{
                Name = name;
                Chemistry = chemistry;
                Compatibility = compatibility;
                Hotness = hotness;
                Intelligence = intelligence;
                Confidence = confidence;
                Athleticism = athleticism;
                Cooperative = cooperative;
                Loving = loving;
                Authentic = authentic;
                AggregateScore = (Chemistry + Compatibility + Hotness + Intelligence + Confidence + Athleticism + Cooperative + Loving + Authentic) / 9.0;
			}

			// in order for a class to implement the IComparable<T> interface, it must define the IComparable<T>.CompareTo member method
			public int CompareTo(Babe other)
			{
				return AggregateScore.CompareTo(other.AggregateScore);
			}

			//System.Console.WriteLine($"[{id:000}]\t{lastName}, {firstName}");
			public override string ToString()
			{
				//return $"{Name,-10}: Chemistry = {Chemistry,2}, Compatibility = {Compatibility,2}, Hotness = {Hotness,2}, Intelligence = {Intelligence,2}, Confidence = {Confidence,2}, Athleticism = {Athleticism,2}, Cooperative = {Cooperative,2}, Loving = {Loving,2}, Authenticity = {Authentic,2}";
                return $"{Name,-10}: {Math.Round(AggregateScore, 2), 2}";
            }
		}

		public class MyBabeComparer : IComparer<Babe>
		{
			// in order for a comparer class to implement the IComparer<T> interface, it must define the IComparer<T>.Compare member method
			public int Compare(Babe left, Babe right)
			{
				//return left.Hotness - right.Hotness;
				return left.CompareTo(right);
			}
		}

		public void List_IComparable_Comparer_Example()
		{
			List<Babe> myBabes = new List<Babe> {
                // Chemistry + Compatibile + Hot + Intelligent + Confidet + Athletic + Cooperative + Affectionate + Authentic
                new Babe("Erin",       7, 6, 7, 8, 9, 8, 6, 4, 5),
				new Babe("Caroline",   8, 6, 8, 8, 7, 7, 4, 6, 4),
				new Babe("Diana",      6, 7, 8, 8, 4, 7, 9, 9, 9),
				new Babe("Abby",       8, 6, 9, 7, 5, 5, 5, 5, 5),
				new Babe("Bori",       9, 6, 7, 8, 7, 6, 8, 6, 6),
				new Babe("Emily",      9, 7, 9, 6, 6, 6, 8, 8, 8),
				new Babe("Andrea",     7, 5, 6, 6, 6, 4, 6, 4, 3),
				new Babe("Yvonne",     4, 4, 7, 7, 3, 3, 3, 3, 4),
				new Babe("Stephanie", 10, 8, 9, 8, 8, 8, 8, 8, 7),
				new Babe("Denise",     3, 5, 6, 6, 5, 4, 4, 3, 5),
				new Babe("Josie",      7, 8, 9, 6, 6, 7, 9, 9, 8),
				new Babe("Abbi",       9, 7, 9, 7, 7, 7, 7, 6, 6),
				new Babe("Sarah",      6, 6, 7, 7, 7, 5, 9, 9, 9),
                new Babe("Catherine",  8, 5, 9, 9, 5, 6, 2, 3, 7)
            };
			myBabes.Sort();
            //myBabes = myBabes.OrderByDescending(b => b);
            myBabes = myBabes.OrderByDescending(b => b).ToList();
			myBabes.ForEach(babe => Console.WriteLine(babe));

            //myBabes.OrderBy()
			myBabes.Sort(new MyBabeComparer())
                ; // Comparer class must implement the IComparer<T> interface, which means it must define the IComparer<T>.Compare member method
		}
	}

	#endregion CONTAINERS

	#region TYPES

	class Types
	{
		// *** 11 *** Nullable Types
		public class NullableTypes
		{
			public struct YNullable<T> where T : struct
			{
				T value;
				bool hasValue;
				public YNullable(T value)
				{
					this.value = value;
					hasValue = true;
				}

				// conversion operators
				// if you can lose data, the cast needs to be explicit - to convert from nullable to non-nullable
				//public static explicit operator T(T? value) { return value.Value; }
				// if the cast cannot lost information, then it can be implicit - to convert from non-nullable to nullable
				//public static implicit operator T?(T value) { return new T?(value); }

				public override string ToString()
				{
					return HasValue ? value.ToString() : string.Empty;
				}
				public T Value { get { return value; } }
				public bool HasValue { get { return hasValue; } }
			}

			void NullableTypes_Example()
			{
				YNullable<int> y = new YNullable<int>(5);
			}
		}

		// Upcast Downcast TypeSafety

		// *** 27 ***  Type Safety
		class Base
		{
			public int baseValue;
			public void BaseMethod()
			{
				Console.WriteLine("BaseMethod()");
			}
		}

		class Derived : Base
		{
			public float derivedValue;
			public void DerivedMethod()
			{
				Console.WriteLine("DerivedMethod()");
			}
		}

		void TypeSafety()
		{
			Base b = new Base();
			Derived d = new Derived();
			b.baseValue = 5;
			d.derivedValue = 7.3f;

			// reference conversion - implicit (safe)
			b = d;
			// no type conversion has occured
			// compiler can't see float derivedValue with b
			// object b was referencing is now up for garbage collection

			// explicit conversion (not safe)
			// d = b;
			// C# is a managed languaged, which means we aren't allowed to perform explicit conversions like this
			// if we want to tell the compiler to do it anyway, we need to explicity cast it (although it will throw an InvalidCastException at runtime)
			d = (Derived)b; // explicit cast

			// *** 28 *** Casts vs Type Conversion
			// cast can cause conversions - changing something into something else
			// most casts don't change anything at all, just looks at data differently

			// type conversion - something at runtime changes bits
			int i = 5;
			float f = i;
		}

		// *** 29 *** Type Conversion Operators
		class Scooter
		{
			public int Mileage { get; set; }
			public static implicit operator Car(Scooter scoot)
			{
				return new Car { Mileage = scoot.Mileage };
			}
		}

		class Car { public int Mileage { get; set; } }

		void TypeConversion()
		{
			Scooter meScooter = new Scooter();
			Car meCar = meScooter;
		}

		// *** 30 *** as Operator - if cast fails, return null
		void asOperator()
		{
			var rand = new Random();
			bool randomBool = rand.Next() % 2 == 0;
			Base b = randomBool ? new Base() : new Derived();
			Derived d = b as Derived;
			if (d == null)
				Console.WriteLine("Oh, well that failed!");
			else
				Console.WriteLine("WE GOT AN OBJECT!");
		}

		// *** 31 *** is Operator
		void isOperator()
		{
			var rand = new Random();
			bool randomBool = rand.Next() % 2 == 0;
			Base b = randomBool ? new Base() : new Derived();
			Derived d = null;
			if (b is Derived)
				d = (Derived)b;
			if (d == null)
				Console.WriteLine("Oh, well that failed!");
			else
				Console.WriteLine("WE GOT AN OBJECT!");
		}

		// *** 32 *** Type Object Pointer - each instance of a class on the heap allocates memory for all class members as well as a SyncBlockIndex (4 bytes) and a TypeObjectPointer (4 bytes) 
		// allows us and the CLR runtime to know the type of our object at runtime - allows for casting
		// type object - for each type, only one static Type object instance for entire program 
		class Cow
		{
			public int mooCount;
			public string name;
			public Cow(string n = "", int m = 0) { name = n; mooCount = m; }
			public void Moo() { mooCount++; }
		}

		void TypeObjectPointer()
		{
			object o = new Cow();
			Cow c = (Cow)o;
			Type t = c.GetType();
			Console.WriteLine(t.FullName);
			Console.WriteLine(t.BaseType);
		}

		// *** 33 *** Sync Block Index - use any object we instantiate as a mutex or way to stop threads from stepping on each other
		void SyncBlockIndex()
		{
			Cow betsy = new Cow();
			lock (betsy)
			{
				Console.WriteLine(betsy.mooCount);
			}
		}

		// *** 34 *** Value Types and Type Object Pointers - structs are sealed - compiler and runtime know type
		// we don't have SBI and TOP for value types
		struct Cat
		{
			public int meowCount;
		}
		void ValueTypesAndTypeObjectPointers()
		{
			Cat c = new Cat();
			c.meowCount = 9;
			c.GetType(); //runtime will box this value, we don't call GetType on c, we call it on a boxed object
		}

		// *** 35 *** Alignment and Packing - compiler smart enough to 'align' the 2 byte chars on the same memory block so the 4 byte int is 'packed' so no memory wasted on stack
		// *** 36 *** sizeof - inherited from C++, compile time operator that returns size in bytes of a type had you instantiated that type, doesn't work for classes
		// layout, alignment, packing - not determined until run time - managed type, determined by JITR (just in time compiler)
		class MeUglyClass
		{
			public char meChar1;
			public int meInt;
			public char meChar2;
		}

		//using System.Runtime.InteropServices;
		//[StructLayout(LayoutKind.Sequential)]
		//[StructLayout(LayoutKind.Auto)]
		[StructLayout(LayoutKind.Explicit)]
		struct Dog
		{
			[FieldOffset(0)]
			public char c;
			[FieldOffset(12)]
			public int age;
			[FieldOffset(6)]
			public char d;
		}

		void AlignmentPacking_sizeof()
		{
			MeUglyClass mug = new MeUglyClass();
			mug.meChar1 = 'y';
			mug.meInt = 35;
			mug.meChar2 = 'a';
			Dog sparky = new Dog();
			sparky.c = 'c';
			sparky.age = 3;
			sparky.d = 'd';

			//Console.WriteLine(sizeof(MeUglyClass)); // wont work
			Console.WriteLine(sizeof(int));
			unsafe { Console.WriteLine(sizeof(Dog)); }
		}

		// *** 37 *** Constructors - we can call constructors for objects within class definitions (but not other methods), base class constructor called before derived class constructor
		class Farm
		{
			//Cow cow1 = new Cow("Betsy");
			//Cow cow2 = new Cow("Georgy");
			// this is syntatic sugar, changes code to:
			Cow cow1;
			Cow cow2;
			public Farm()
			{
				cow1 = new Cow("Betsy");
				cow2 = new Cow("Georgy");
				Console.WriteLine("Farm()");
			}
		}

		// *** 38 *** Garbage Collection - collect cleans up null objects and compacts heap
		class IntWrapper
		{
			public int wrappedInt;
		}

		void GarbageCollection()
		{
			var wrapped1 = new IntWrapper();
			var wrapped2 = new IntWrapper();
			var wrapped3 = new IntWrapper();
			wrapped1.wrappedInt = 1;
			wrapped2.wrappedInt = 2;
			wrapped3.wrappedInt = 3;
			GC.Collect();
			wrapped2 = null;
			GC.Collect();  // GC collects and cleans wrapped2, then compacts heap
		}

		// *** 39 *** Heap
		class CountingClass
		{
			static short counter = 0;
			short id;
			char c;
			public CountingClass()
			{
				id = counter;
				c = (char)(((short)'a') + counter);
				counter++;
			}
		}

		void Heap()
		{
			GC.Collect();
			var c1 = new CountingClass();
			var c2 = new CountingClass();
			new CountingClass();
			new CountingClass();
			var c5 = new CountingClass();
			GC.Collect(); // vaporizes two unassigned instantiations
		}

		// *** 40 *** Stack - immune to GC.Collect()
		// *** 41 *** Call Stack - we instantiate methods in memory just like we do with objects
		// *** 42 *** Stack Debugger - each instance of Call Stack - stack frame (activation instance)
		// *** 43 *** StackOverflowException
		// *** 44 *** Stacks and Variable Memory Sharing

		// STATIC
		// *** 45 *** Static Variables - data that is shared amongst all instances of a class (why not use keyword shared?), one of first types of data when programming in infancy, way before objects
		// static analysis is compile time analysis
		// static variables are not on the stack or the heap (dynamic - can grow and shrink), but in the static area
		// *** 46 *** Static Data
		// *** 47 *** Static Constructors - run the first time you do anything with the type
		// *** 48 *** Static Constructors and Exceptions - static constructor only gets called once
		// *** 49 *** Static's Roots - static data is memory created by the compiler, exists before and after Main()
		// *** 50 *** Static Classes - cannot call static methods directly on instance variable, static classes cannot be instantiated, no instances are created
		//                      syntatic sugar provided by the C# compiler - static classes not native to .NET - cannot inherit from sealed classes, cannot create instance of abstract classes
		// *** 51 *** Static Classes vs Singleton Design Pattern - creates one instance of a class (static classes do not and cannot have an instance)

		class StaticClass
		{
			public static int numInstances;
			static int whatever;
			int id;
			public StaticClass()
			{
				id = ++numInstances;
			}

			static StaticClass()
			{
				numInstances = new Random().Next(20);
				whatever = numInstances + 5;
				throw new Exception();
			}
		}

		void StaticVariablesStaticConstructors()
		{
			StaticClass.numInstances = 100; // calls static constructor
			StaticClass one = new StaticClass();
			StaticClass two = new StaticClass();
		}

		void StaticConstructorsExceptions()
		{
			try
			{
				new StaticClass();
			}
			catch { }
			new StaticClass();  // StaticClass is dead to me because constructor throws an exception
		}

		// cannot call static methods directly on instance variable
		static class Logger // syntatic sugar provided by the C# compiler - static classes not native to .NET
		{
			static StreamWriter outStream;
			static int logNumber = 0;

			static public void InitializeLogging() { outStream = new StreamWriter("myLog.txt"); }
			static public void ShutdownLogging() { outStream.Close(); }
			static public void LogMessage(string message) { outStream.WriteLine(logNumber++ + ": " + message); }
		}

		// Singleton Design Pattern
		class Log
		{
			StreamWriter outStream;
			int logNumber = 0;

			//static Log theInstance = new Log();  // eager loading
			static Log theInstance;

			Log() { }
			public void InitializeLogging() { outStream = new StreamWriter("myLog.txt"); }
			public void ShutdownLogging() { outStream.Close(); }
			public void LogMessage(string message) { outStream.WriteLine(logNumber++ + ": " + message); }

			//public static Log TheInstance { get { return theInstance; } }

			// lazy loading
			public static Log TheInstance
			{
				get
				{
					if (theInstance == null)
						theInstance = new Log();
					return theInstance;
				}
			}
		}

		// classes with static methods and static variables are never meant to be instantiated
		void StaticClasses()
		{
			//Logger l = new Logger(); // since Logger is static, both statements are invalid
			Logger.InitializeLogging();
			Logger.LogMessage("I love static data");
			Logger.LogMessage("static data exists before and after main()");
			Logger.LogMessage("When I think static, I think memory created by the compiler");
			Logger.ShutdownLogging();
		}

		void SingletonDesignPattern()
		{
			Log.TheInstance.InitializeLogging();
			Log.TheInstance.LogMessage("I love static data");
			Log.TheInstance.LogMessage("static data exists before and after main()");
			Log.TheInstance.LogMessage("When I think static, I think memory created by the compiler");
			Log.TheInstance.ShutdownLogging();
		}

		// *** 54 *** Compile Time Types vs Runtime Types
		class Fruit { }
		class Papaya : Fruit { }
		class Orange : Fruit { }

		void CompileTimeTypesVsRuntimeTypes()
		{
			Random rand = new Random();
			bool randomBool = rand.Next() % 2 == 0;
			Fruit a = randomBool ? (Fruit)new Papaya() : (Fruit)new Orange();
			Papaya d = (Papaya)a; // this will throw an invalid cast exception half the time
		}

		// *** 55 *** this Constructor
		class Human
		{
			public string Name { get; set; }
			public int Age { get; set; }
			public Human(string name = "")
			{
				if (name.Length > 10)
					throw new Exception();
				Name = name;
			}
			public Human(string name = "", int age = 10) : this(name) // will call this constructor first
			{
				Age = age;
			}
		}

		void thisConstructor()
		{
			Human h = new Human("Yianni", 35);
		}
	}

	// *** 52 *** Namespaces
	//using Dogg = Yianni.Junk.Morejunk.Doggy;
	namespace Yianni.Junk.Morejunk
	{
		class Doggy { }
	}

	#endregion TYPES

	#region DELEGATES_LAMBDA_EXPRESSIONS_EVENTS

	class DelegatesLambdaExpressionsEvents
	{
		// *** 1 *** Hello World Delegates
		class HelloWorldDelegatesMainClass
		{
			delegate void MeDelegate();
			/* underlying MSIL - compiler just turns it into class 
             * class MeDelegate { }
             * a delegate is a reference to an object that is a method/function
             * treating methods/functions as first class objects - we have a reference to them
             * a method is an address where execution needs to start
             * a delegate keeps track of 2 things: method and target - instance to invoke it (the method) upon
             * delegates allow us to parameterize/pass code - to parameterize our code - a reference to our code
             * 2 generic delegate types - Func and Action - Func returns value, Action returns void
             */
			static void HelloWorldDelegatesMain()
			{
				// we have to pass a method to the delegate constructor
				//MeDelegate del = new MeDelegate(Foo);
				MeDelegate del = Foo; // compiler turns into: MeDelegate del = new MeDelegate(Foo);
				del(); // this reference is treated as a method - compiler turns into:
				del.Invoke(); // which invokes the method del references: Foo()
				InvokeTheDelegate(del);
				InvokeTheDelegate(Foo);
				InvokeTheDelegate(new MeDelegate(Goo));
			}
			static void InvokeTheDelegate(MeDelegate deler) { deler(); }
			static void Foo() { Console.WriteLine("Foo()"); }
			static void Goo() { Console.WriteLine("Goo()"); }
		}

		// *** 2 *** Delegates - Method and Target
		// method is an address where execution needs to start
		// a delegate keeps track of 2 things: method and target - instance to invoke it (the method) upon
		class DelegatesMethodAndTargetMainClass
		{
			delegate void MeDelegate(int value);

			static void DelegatesMethodAndTargetMain()
			{
				MeDelegate d = Foo;
				Console.WriteLine(d.Method); // the method to invoke: Void Foo(Int32)
				Console.WriteLine(d.Target); // the object to invoke it upon: nothing printed here because target is null since Foo is a static method

				new DelegatesMethodAndTargetMainClass().Goo(5);
				DelegatesMethodAndTargetMainClass m = new DelegatesMethodAndTargetMainClass();
				m.Goo(5);
				d = m.Goo; //<d.Target>.<d.Method>
				Console.WriteLine(d.Method); // the method to invoke: Void Goo(Int32)
				Console.WriteLine(d.Target); // the object to invoke it upon: DelegatesMethodAndTargetMainClass
			}
			static void Foo(int f) { } // static method
			void Goo(int g) { } // instance method
		}

		// *** 3 *** Why Delegates - delegates allow us to parameterize code, i.e. to pass a method reference as a parameter
		class WhyDelegatesMainClass
		{
			delegate bool MeDelegate(int n);
             
			static bool LessThanFive(int n) { return n < 5; }
			static bool LessThanTen(int n) { return n < 10; }
			static bool GreaterThanThirteen(int n) { return n > 13; }

			static void WhyDelegatesMain()
			{
				int[] numbers = new[] { 2, 7, 3, 9, 17, 5, 7, 1, 8, 13 };
				//IEnumerable<int> result = RunNumbersThroughGauntlet(numbers, LessThanFive);
				//IEnumerable<int> result = RunNumbersThroughGauntlet(numbers, LessThanTen);
				IEnumerable<int> result = RunNumbersThroughGauntlet(numbers, GreaterThanThirteen);
				foreach (int n in result)
					Console.WriteLine(n);
			}

			// we re-define the method to take the comparison functionality method (MeDelegate gauntlet) as a parameter
			// the guantlet delegate/parameter can be any method that takes an int and returns a bool, as defined by the delegate signature/definition for MeDelegate: delegate bool MeDelegate(int n);
			static IEnumerable<int> RunNumbersThroughGauntlet(IEnumerable<int> numbers, MeDelegate gauntlet)
			{
				foreach (int number in numbers)
					if (gauntlet(number))
						yield return number;
			}
		}

		// *** 4 *** Lambda Expressions
		// lambda - points to body of expression, compiler creates method available to us - method without a name - it's a one-off - anonymous function used to create delegates
		class LambdaExpressionsMainClass
		{
			delegate bool MeDelegate(int n);

			static void LambdaExpressionsMain()
			{
				int[] numbers = new[] { 2, 7, 3, 9, 17, 5, 7, 1, 8, 13 };
				//IEnumerable<int> result = RunNumbersThroughGauntlet(numbers, n => n < 5);
				//IEnumerable<int> result = RunNumbersThroughGauntlet(numbers, n => n < 10);
				IEnumerable<int> result = RunNumbersThroughGauntlet(numbers, n => n > 13); // this way we do not have to define each method LessThanFive, LessThanTen, GreatherThanThirteen - use lambdas to define what we want inline
				foreach (int n in result)
					Console.WriteLine(n);
			}

			static IEnumerable<int> RunNumbersThroughGauntlet(IEnumerable<int> numbers, MeDelegate gauntlet)
			{
				foreach (int number in numbers)
					if (gauntlet(number))
						yield return number;
			}
		}

		// *** 5 *** Delegate Chaining - useful for events and event listeners
		// a delegate can reference a chain of methods - delegate chains are immutable, like strings, we can create new ones all we want, but can't mutate existing ones
		// each time you chain on a new delegate, it first creates a copy of the old chain, then adds a reference to the instance of the newly chained delegate
		// chaining used for events - observer pattern - add in as many observers as we want
		class DelegateChainingMainClass
		{
			delegate void MeDelegate();

			static void DelegateChainingMain() // Delegate -> MulticastDelegate -> MeDelegate
			{
				MeDelegate d = Foo;
				d += Goo; // syntatic sugar for: d = (MeDelegate)Delegate.Combine(d, new MeDelegate(Goo));
				d += Sue;
				d += Foo;
				d -= Foo;
				foreach (MeDelegate m in d.GetInvocationList())
					Console.WriteLine(m.Target + ": " + m.Method);
				d();
			}

			static void Foo() { Console.WriteLine("Foo()"); }
			static void Goo() { Console.WriteLine("Goo()"); }
			static void Sue() { Console.WriteLine("Sue()"); }
		}

		// *** 6 *** MulticastDelegate vs Delegate
		// Delegate -> MulticastDelegate -> MeDelegate
		// initially there was a distinction - if delegate returns void it is a MulticastDelegate (which means you can chain)
		// otherwise if it returns something it's a normal Delegate
		// ended up being ALL delegates are going to be MulticastDelegate
		class MulticastDelegateVsDelegateMainClass
		{
			delegate int MeDelegate();

			static void MulticastDelegateVsDelegateMain()
			{
				MeDelegate d = ReturnFive;
				d += ReturnTen;
				int value = d();
				Console.WriteLine(value); // prints 10 (latest value)

				d += Return22;

				// can extract ints.Add() foreach MeDelegate.GetInvocationList into separate method GetAllReturnValues
				foreach (int i in GetAllReturnValues(d)) // prints 5, 10, 22
					Console.WriteLine(i);
			}

			static List<int> GetAllReturnValues(MeDelegate d)
			{
				List<int> ints = new List<int>();
				foreach (MeDelegate del in d.GetInvocationList())
					ints.Add(del());
				return ints;
			}
			static int ReturnFive() { return 5; }
			static int ReturnTen() { return 10; }
			static int Return22() { return 22; }
		}

		// *** 7 *** Func and Action - generic delegate types
		class FuncAndActionMainClass
		{
			delegate T GenericMeDelegate<T>(); // generified - can take any type - all delegate types can be superimposed or satisifed with a few generic delegate types

			static void FuncAndActionMain()
			{
				// Generic version
				GenericMeDelegate<int> g = ReturnFive;
				g += ReturnTen;
				g += Return22;
				foreach (int i in GenericGetAllReturnValues(g))
					Console.WriteLine(i);

				// Func version
				Func<int> d = ReturnFive;
				d += ReturnTen;
				d += Return22;
				foreach (int i in FuncGetAllReturnValues(d))
					Console.WriteLine(i);

				Func<int, bool> f = TakeAnIntAndReturnBool;
				Action<string> s = TakeStringAndReturnVoid;
			}

			static int ReturnFive() { return 5; }
			static int ReturnTen() { return 10; }
			static int Return22() { return 22; }

			// Generic version
			static IEnumerable<TArg> GenericGetAllReturnValues<TArg>(GenericMeDelegate<TArg> d)
			{
				foreach (GenericMeDelegate<TArg> del in d.GetInvocationList())
					yield return del();
			}

			// Func version
			static IEnumerable<TArg> FuncGetAllReturnValues<TArg>(Func<TArg> d)
			{
				foreach (Func<TArg> del in d.GetInvocationList())
					yield return del();
			}

			static bool TakeAnIntAndReturnBool(int i) { return false; }
			static void TakeStringAndReturnVoid(string s) { }
		}

		// *** 8 *** Delegate Chains and Exceptions
		class DelegateChainsAndExceptionsMainClass
		{
			public static void DelegateChainsAndExceptionsMain()
			{
				// produces error:
				//Action del = Moo + BeNaughty + Goo;
				Action del = (Action)Moo + BeNaughty + Goo; // converts to //Action del = new Action(Moo) + BeNaughty + Goo;
				foreach (Action a in del.GetInvocationList())
				{
					try {
						a();
					} catch { }
				}
			}
			static void Moo() { Console.WriteLine("Moo()"); }
			static void BeNaughty() { throw new Exception(); }
			static void Goo() { Console.WriteLine("Goo()"); }
		}

		// *** 9 *** Anonymous Methods vs Lambda Expressions - lambdas are just better, much more terse
		class AnonymousMethodsVsLambdaExpressionsMainClass
		{
			public static void AnonymousMethodsVsLambdaExpressionsMain()
			{
				Func<int, bool> func = i => i > 5;
				Console.WriteLine(func(3));
				Console.WriteLine(func(7));

				Func<int, bool> anonMethod = delegate (int i) { return i > 5; }; // Anonymous Methods C# 2.0, lambda is much better: i => i > 5; Compiler unwraps both to normal member method 
				Console.WriteLine(anonMethod(3));
				Console.WriteLine(anonMethod(7));
			}
		}

		// *** 10 & 11 *** Closures, How The Compiler Implements Closures - came out in .NET 3.0 - Closures are kept separate between multiple Action instances
		class ClosuresMainClass
		{
			public static void ClosuresMain()
			{
				int i = 0;
				Action a = () => i++;  // lambda expression defines a method, i is in scope of lambda expression, but i is defined in ClosuresMain
				a(); a(); a();
				Console.WriteLine(i); // prints 3

				Action b = GiveMeAction();
				b(); b(); b(); // prints 3, i's scope is continuing on

				Action c = GiveMeActionChain();
				c(); c(); c(); // prints F 0, S 1, F 2, S 3, F 4, S 5

				// closures are kept separate between mulitple Action instances - d and e each have different versions of i
				Action d = GiveMeAction();
				d();
				Action e = GiveMeAction();
				e(); d(); d(); e(); d();
			}

			// Functional Programming, function that returns a function
			// this is a closure because we are closing it on i's scope; captures i's scope - this function captures i's scope, it's a Closure
			static Action GiveMeAction()
			{
				int i = 0;
				return () => i++; //return new Action(() => i++);
			}

			// i is sealed in a Closure
			static Action GiveMeActionChain()
			{
				Action ret = null;
				int i = 0;
				ret += () => { Console.WriteLine("First Method " + i++); };
				ret += () => { Console.WriteLine("Second Method " + i++); };
				return ret;
			}
		}

		// *** 12 *** More Complex Closures
		class MoreComplexClosuresMainClass
		{
			class DisplayClass
			{
				int i = 0;
				public void NamelessFunction1() { i++; }
				public void NamelessFunction2() { i += 2; }
			}
			static Action GiveMeAction2()
			{
				Action ret = null;
				ret += new DisplayClass().NamelessFunction1;
				ret += new DisplayClass().NamelessFunction2;
				return ret;
			}
			static Action GiveMeAction3()
			{
				Action ret = null;
				var temp = new DisplayClass();
				ret += temp.NamelessFunction1;
				ret += temp.NamelessFunction2;
				return ret;
			}
		}

		// *** 13 *** Basic Observer Pattern Using Delegates
		class BasicObserverPatternUsingDelegatesMainClass
		{
			static void BasicObserverPatternUsingDelegatesMain()
			{
				TrainSignal trainSignal = new TrainSignal();
				new Car(trainSignal);
				new Car(trainSignal);
				new Car(trainSignal);
				new Car(trainSignal);
                // each of the four Cars is now subscribed, they are observing the trainSignal
                trainSignal.HereComesATrain(); // invokes the TrainsAComing Action(delegate), and everyone subscribed to this Action(delegate) will receive the message - 4 x Screeetch
                
			}
			class TrainSignal
			{
				public Action TrainsAComing;
				public void HereComesATrain()
				{
					// turn on the train signal lights, lower the bar, etc...
					TrainsAComing();
				}
			}
			// Car is observing a trainSignal
			class Car
			{
				public Car(TrainSignal trainSignal)
				{
					trainSignal.TrainsAComing += StopTheCar; // Car is observing this trainSignal, when invoked, add the method StopTheCar to list of subscribers
				}
				void StopTheCar() { Console.WriteLine("Screeetch"); }
			}
		}

		// *** 14 *** Events vs Delegates - event is a delegate reference with 2 restrictions:
		// 1) Cannot invoke delegate reference directly
		// 2) Cannot assign to it directly, but can add to += and remove from -=

		// *** 15 *** How the Compiler Implements Events - events are native to .net assemblies - makes the field private and then adds the add and remove methods and some metadata
		// event keyword causes compiler to create add and remove methods in the background and then privates the field so we can't directly invoke it

		// *** 16 *** Events Add and Remove
		class EventsAddRemoveMainClass
		{
			class Cow
			{
				private Action mooing; // delegate reference
				public event Action Mooing // event type is Action, ie Action is delegate type for our event
				{
					add { mooing += value; }  // invoked by +=
					remove { mooing -= value; }  // invoked by -=
				}
				public void PushSleepingCow()
				{
					if (mooing != null)
						mooing();
				}
			}
			static void EventsAddRemoveMain()
			{
				Cow c = new Cow();
				c.Mooing += () => Console.WriteLine("Giggle"); // the += invokes the above add method
				c.PushSleepingCow(); // will print out 'Giggle'
			}
		}

		// *** 17, 18 *** EventHandler and sender, EventArgs - methods that subscribe to events are handlers
		public class EventHandlerEventArgsMainClass
		{
			public enum CowState { Awake, Sleeping, Dead }

			class CowTippedEventArgs : EventArgs
			{
				public CowState CurrentCowState { get; private set; }
				public CowTippedEventArgs(CowState currentState) { CurrentCowState = currentState; }
			}

			class Cow
			{
				public string Name { get; set; }
				//public event Action Moo; // we are replacing Action with EventHandler
				public event EventHandler Moo; // EventHandler takes in 2 objects: object sender and EventArgs e
				public void BeTippedOver()
				{
					if (Moo != null)
						Moo(this, EventArgs.Empty);
				}

				public event EventHandler<CowTippedEventArgs> MooEventArgs; // if we want to define some custom EventArgs
				public void BeTippedOverEventArgs()
				{
					if (MooEventArgs != null)
						MooEventArgs(this, new CowTippedEventArgs(CowState.Awake));
				}
			}

			static void EventHandlerEventArgsMain()
			{
				Cow c1 = new Cow { Name = "Betsy" };
				c1.Moo += giggle; // we can subscribe giggle handler method to our event Moo because giggle has same params as EventHandler
				Cow c2 = new Cow { Name = "Georgy" };
				c2.Moo += giggle;
				Cow victim = new Random().Next() % 2 == 0 ? c1 : c2;
				victim.BeTippedOver();

				Cow c3 = new Cow { Name = "Sally" };
				c3.MooEventArgs += giggleEventArgs; // we can subscribe giggleEventArgs handler method to our event MooEventArgs because giggleEventArgs has same params as EventHandler<CowTippedEventArgs>
				Cow c4 = new Cow { Name = "Irene" };
				c4.MooEventArgs += giggleEventArgs;
				Cow victimEventArgs = new Random().Next() % 2 == 0 ? c3 : c4;
				victimEventArgs.BeTippedOverEventArgs();
			}

			static void giggle(object sender, EventArgs e) // handler method that can handle Moo
			{
				Cow c = sender as Cow;
				Console.WriteLine("Giggle giggle... We made " + c.Name + " moo!");
			}

			static void giggleEventArgs(object sender, CowTippedEventArgs e) // handler method that can handle MooEventArgs
			{
				Cow c = sender as Cow;
				Console.WriteLine("Giggle giggle... We made " + c.Name + " moo!");
				switch (e.CurrentCowState)
				{
					case CowState.Awake:
						Console.WriteLine("Run!");
						break;
					case CowState.Sleeping:
						Console.WriteLine("Tickle it");
						break;
					case CowState.Dead:
						Console.WriteLine("Butcher it");
						break;
				}
			}
		}

		// *** 19 *** Events Example
		public class EventsExampleMainClass
		{
			static void EventsExampleMain()
			{
				var but1 = new Button { Text = "Button 1" };
				var but2 = new Button { Text = "Button 2" };
				//but1.Click += (s, e) => MessageBox.Show("You clicked the button");
				but1.Click += theButtonWasClicked;
				but2.Click += theButtonWasClicked;
				but1.MouseMove += mouseMoved;

				var form = new Form();
				but2.Top = 100;
				form.Controls.Add(but1);
				form.Controls.Add(but2);
				//form.Show();
				Application.Run(form);
			}

			static void theButtonWasClicked(object sender, EventArgs e) // our handler method
			{
				Button b = (Button)sender;
				MessageBox.Show("You clicked the button: " + b.Text);
			}

			static void mouseMoved(object sender, MouseEventArgs e)
			{
				Button b = (Button)sender;
				MessageBox.Show("You moved the mouse: " + e.Location + " over the button: " + b.Text);
			}
		}

		// *** 20 *** Types With Many Events
		// backing field is 4 bytes (delegate references are 4 bytes) for each EventHandler
		// underlying data structure containing list of subscribers of EventHandlers
		class TypesWithManyEventsMainClass
		{
			class Cow
			{
				private Dictionary<string, EventHandler> subscribers = new Dictionary<string, EventHandler>(); // store subscribing delegate references on demand
				const string BeginMooKey = "Begin moo";
				public event EventHandler BeginMoo
				{
					add { addSubscriber(BeginMooKey, value); }
					remove { removeSubscriber(BeginMooKey, value); }
				}

				void addSubscriber(string key, EventHandler subscriber)
				{
					if (subscribers.ContainsKey(key))
						subscribers[key] += subscriber;
					else
						subscribers.Add(key, subscriber);
				}
				void removeSubscriber(string key, EventHandler subscriber)
				{
					if (!subscribers.ContainsKey(key))
						return;
					subscribers[key] -= subscriber;
					if (subscribers[key] == null)
						subscribers.Remove(key);
				}

                /* do not need all these EventHandlers if we store delegate references on demand using above subscribers Dictionary
                // hook design pattern
				public event EventHandler Moo;
				public event EventHandler EndMoo;
				public event EventHandler BeginWalking;
				public event EventHandler Walk;
				public event EventHandler EndWalking;
				public event EventHandler BeginSleeping;
				public event EventHandler Sleeping;
				public event EventHandler EndSleeping;
                */
			}
		}

		// *** 21 *** Unsubscribe Events
		// if you don't unsubscribe, you cause 2 problems:
        // 1) events will still subscribe so code behind event will run
        // 2) maintains a reference to the delegate which you subscribed
		// delegate has a reference to an object you might not be interested in anymore. 
        // if you never unsubscribe, GC will never come clean up and you will run out of memory
		public class UnsubscribeEventsMainClass
		{
			class Person
			{
				Button myButton;
				public Person(Button but)
				{
					myButton = but;
					but.Click += cliffDive;
				}
				void cliffDive(object sender, EventArgs e)
				{
					Console.WriteLine("Jumping!");
					myButton.Click -= cliffDive;
				}
			}

			static void UnsubscribeEventsMain()
			{
				var but = new Button { Text = "Jump!" };
				new Person(but);
				var form = new Form();
				form.Controls.Add(but);
				Application.Run(form);
			}
		}

		// *** 22 *** Delegate Contravariance
		public class DelegateContravarianceMainClass
		{
			class Base { }
			class Derived : Base { }

			delegate void MyDelegate(Base b);
			delegate void MyOtherDelegate(Derived d);

			static void TakeDerived(Derived d) { }
			static void TakeBase(Base b) { }
			static void DelegateContravarianceMain()
			{
				//MyDelegate d1 = TakeDerived; // MyDelegate requires general Base, but method TakeDerived requires specific Derived - the argument Base is not contravariant with Derived
				//d1(new Derived()); // this actually works because of TakeDerived takes a Derived
				MyDelegate d2 = TakeBase;

				MyOtherDelegate d3 = TakeDerived;
				//d3(new Base()); // will not work because TakeDerived requires more specific Derived object
				MyOtherDelegate d4 = TakeBase; // works because MyOtherDelegate Derived argument is a more specific type of TakeBase Base argument - the argument Derived is contravariant with Base
			}
		}

		// *** 23 *** Delegate Covariance - same concept as contravariance except with return types instead of parameters
		public class DelegateCovarianceMainClass
		{
			class Base { }
			class Derived : Base { }

			delegate Base ReturnsBaseDelegate();
			delegate Derived ReturnsDerivedDelegate();

			static Base ReturnsBase() { return new Base(); }
			static Derived ReturnsDerived() { return new Derived(); }
			static void DelegateCovarianceMain()
			{
				ReturnsBaseDelegate b;
				b = ReturnsBase;
				b = ReturnsDerived; // works because Derived is a more specific type of Base - they are covariant

				ReturnsDerivedDelegate d;
				d = ReturnsDerived;
				//d = ReturnsBase; // has the wrong return type as ReturnsDerivedDelegate d expecting specific return type of Derived, but Base is too general
			}
		}
	}

	#endregion DELEGATES_LAMBDA_EXPRESSIONS_EVENTS
	
	#region YIELD_STATEMENT

	// *** 1 *** yield Statement Introduction
	public class YieldStatementIntroductionMainClass
	{
		static Random rand = new Random();
		static IEnumerable<int> GetRandomNumbers(int count)
		{
			//List<int> ints = new List<int>(count);
			//for (int i = 0; i < count; i++)
			//    ints.Add(rand.Next());
			//return ints;

			// this replaces above code
			for (int i = 0; i < count; i++)
				yield return rand.Next(); // syntactic sugar - kind of like this method sticks around and returns several values instead of one
		}

		static void YieldStatementIntroductionMain()
		{
			foreach (int num in GetRandomNumbers(10))
				Console.WriteLine(num);
		}
	}

	// *** 2 *** yield's Syntactic Sugar
	public class YieldSyntacticSugarMainClass
	{
		static Random rand = new Random();
		static IEnumerable<int> GetRandomNumbers(int count)
		{
			for (int i = 0; i < count; i++)
				yield return rand.Next();
		}
		static void YieldSyntacticSugarMain()
		{
			foreach (int num in GetRandomNumbers(10))
				Console.WriteLine(num);
		}
	}

	// *** 3 *** yield return - Writing the Compiler's Syntactic Sugar
	public class YieldReturnMainClass
	{
		static Random rand = new Random();
		class GetRandomNumbersClass : IEnumerable<int>, IEnumerator<int>
		{
			public int count;
			public int i;
			int current; // stores current random number
			int state; // way this object keeps track of what happened when

			// need these to implement IEnumerator<int> interface
			public bool MoveNext()
			{
				// switch below mimics this code
				//for (int i = 0; i < count; i++)
				//    yield return rand.Next();
				switch (state)
				{
					case 0:
						i = 0;
						goto case 1;
					case 1:
						state = 1;
						if (!(i < count))
							return false;
						current = YieldReturnMainClass.rand.Next();
						state = 2; // have to prepare for next time MoveNext is called
						return true;
					case 2:
						i++;
						goto case 1;
				}
				return false;
			}

			public int Current // this version is not explicitly implemented
			{
				get { return current; }
			}
			object IEnumerator.Current
			{
				get { return Current; } // ends up resulting to the non-explicitly implemented Current above
			}
			public void Reset() { throw new NotImplementedException(); }
			public void Dispose() { }
			// need these to implement IEnumerable<int> interface
			public IEnumerator<int> GetEnumerator() // this version is not explicitly implemented - not only IEnumerable, but also IEnumerator
			{
				return this;
			}
			IEnumerator IEnumerable.GetEnumerator() // this version is explicitly implemented
			{
				return GetEnumerator(); // ends up resulting to the non-explicitly implemented GetEnumerator above
			}
		}
		
		static IEnumerable<int> GetRandomNumbers(int count)
		{
			GetRandomNumbersClass ret = new GetRandomNumbersClass();
			ret.count = count;
			return ret;
			//for (int i = 0; i < count; i++) // this for loop needs to end up in above MoveNext, and all the variables in this for loop need to become data members of GetRandomNumbersClass
			//    yield return rand.Next();
		}

		static void YieldReturnMain()
		{
			foreach (int num in GetRandomNumbers(10))
				Console.WriteLine(num);
		}
	}

	// *** 4 *** yield return - Another Syntactic Sugar Example
	public class AnotherYieldReturnMainClass
	{
		class NumbersHybrid : IEnumerable<int>, IEnumerator<int>
		{
			int state;
			int current;
			public bool MoveNext()
			{
				switch (state)
				{
					case 0:
						Console.WriteLine("Begin");
						Console.WriteLine("yielding 3");
						current = 3;
						state = 1;
						break;
					case 1:
						Console.WriteLine("yielding 5");
						current = 5;
						state = 2;
						break;
					case 2:
						Console.WriteLine("yielding 1");
						current = 1;
						state = 3;
						break;
					case 3:
						Console.WriteLine("End!!!");
						return false;
				}
				return true;
			}

			public int Current
			{
				get { return current; }
			}

			object IEnumerator.Current { get { return Current; } }
			public void Reset() { throw new NotImplementedException(); }
			public void Dispose() { }
			public IEnumerator<int> GetEnumerator() { return this; }
			IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		}

		static IEnumerable<int> Numbers
		{
			get { return new NumbersHybrid(); }
		}

		static void AnotherYieldReturnMain()
		{
			IEnumerable<int> source = Numbers;
			IEnumerator<int> rator = source.GetEnumerator();
			while (rator.MoveNext())
				Console.WriteLine(rator.Current);

			// syntactic sugar for above snippet
			//foreach (int i in Numbers)
			//    Console.WriteLine(i);
		}

		// de-sugarize below example
		/*static IEnumerable<int> Numbers
        {
            get
            {
                Console.WriteLine("Begin");
                Console.WriteLine("yielding 3");
                yield return 3;
                Console.WriteLine("yielding 5");
                yield return 5;
                Console.WriteLine("yielding 1");
                yield return 1;
                Console.WriteLine("End!!!");
            }
        }*/
	}

	// *** 5 *** yield break - terminates iteration early
	public class YieldBreakMainClass
	{
		static Random rand = new Random();

		static IEnumerable<int> GetRandomNumbers() // i want random numbers until i quit asking for them
		{
			while (true)
			{
				int num = rand.Next();
				if (num % 100 == 0)
					yield break;
				yield return num;
			}
		}
		static void YieldBreakMain()
		{
			foreach (int i in GetRandomNumbers())
				Console.WriteLine(i);
		}

		/*static IEnumerable<int> GetRandomNumbers() // i want random numbers until i quit asking for them
        {
            while(true)
                yield return rand.Next();
        }
        static void YieldBreakMain()
        {
            foreach (int i in GetRandomNumbers())
            {
                if (i % 100 == 0)
                    break;
                Console.WriteLine(i);
            }
        }*/
	}

	// *** 6 *** Compiler's Version of yield Implementation - allow for exceptions to be thrown between yield returns
	public class CompilerVersionYieldImplementationMainClass
	{
		static IEnumerable<int> GetRandomNumbers()
		{
			yield return 9;
			yield return 2;
			Console.WriteLine("Hello, blowing up!");
			if (new Random().Next() == 8)
				throw new Exception("Kaboom!");
			yield return 5;
		}

		static void CompilerVersionYieldImplementationMain() { }
	}

	#endregion YIELD_STATEMENT

	#region GENERICS

	// *** 1 *** Generics Intro
	public class GenericsIntro
	{
		class Pair<T, U>
		{
			public T First { get; set; }
			public U Second { get; set; }
			public override string ToString()
			{
				return "{ " + First + ", " + Second + "}";
			}
		}

		class GenericsIntroMainClass
		{
			static void GenericsIntroMain()
			{
				Pair<int, int> p = new Pair<int, int> { First = 5, Second = 20 };
				Pair<string, string> marriage1 = new Pair<string, string> { First = "Suzy", Second = "Bob" };
				Pair<string, string> marriage2 = new Pair<string, string> { First = "Fred", Second = "Bill" };
				Pair<string, string> marriage3 = new Pair<string, string> { First = "Samantha", Second = "Rover" };
				Console.WriteLine(marriage1.ToString());
			}
		}
	}

	// *** 2 *** Generics and Code Bloat - compiler has to create underlying code for each combination of types

	// *** 3, 4 *** Basic Generic Data Structure, Generic Methods
	public class BasicGenericDataStructureGenericMethods
	{
		class List<T>
		{
			T[] items = new T[50];
			int currentIndex;

			public void Add(T i)
			{
				if (currentIndex == items.Length)
					Grow();
				items[currentIndex++] = i;
			}
			public T Get(int index)
			{
				return items[index];
			}
			void Grow()
			{
				T[] temp = new T[items.Length * 2];
				Array.Copy(items, temp, items.Length);
				items = temp;
			}

			public int Length { get { return items.Length; } }
		}

		class BasicGenericDataStructureGenericMethodsMainClass
		{
			static void PrintItems<T>(List<T> items)
			{
				for (int i = 0; i < items.Length; i++)
					Console.WriteLine(items.Get(i));
			}
			static void BasicGenericDataStructureGenericMethodsMain()
			{
				List<int> myInts = new List<int>();
				myInts.Add(12); myInts.Add(25); myInts.Add(92);
				myInts.Add(5);
				PrintItems(myInts);
				List<string> myStrings = new List<string>();
				myStrings.Add("Bob");
				myStrings.Add("Suzy");
				myStrings.Add("Fred");
				PrintItems(myStrings);
			}
		}
	}

	// *** 5 *** Built-In Generic Data Structures - generics are native to the .NET runtime
	public class BuiltInGenericDataStructures
	{
		class BuiltInGenericDataStructuresMainClass
		{
			static void BuiltInGenericDataStructuresMain()
			{
				List<int> myList = new List<int>();
				myList.Add(5); myList.Add(10);
				myList.Add(20);
				for (int i = 0; i < myList.Count; i++)
					Console.WriteLine(myList[i]);

				ArrayList myArrayList = new ArrayList();
				myArrayList.Add(5); myArrayList.Add("jamie");
				myList.Add(20);
				for (int i = 0; i < myArrayList.Count; i++)
					Console.WriteLine(myArrayList[i]);
			}
		}
	}

	// *** 6 *** Generic Type Instantiation
	public class GenericTypeInstantiation
	{
		class MyClass<T>
		{
			public static int Value;
			static MyClass()
			{
				Console.WriteLine(typeof(MyClass<T>));
			}
		}

		class GenericTypeInstantiationMainClass
		{
			static void GenericTypeInstantiationMain()
			{
				MyClass<string>.Value = 10;
				MyClass<char>.Value = 20;
				MyClass<GenericTypeInstantiationMainClass>.Value = 53;
				Console.WriteLine(MyClass<string>.Value);
				Console.WriteLine(MyClass<char>.Value);
				Console.WriteLine(MyClass<GenericTypeInstantiationMainClass>.Value);
			}
		}
	}

	// *** 7 *** Generic Open and Closed Types - open type is a type that takes a generic argument
	public class GenericOpenAndClosedTypes
	{
		class MyClass<T>
		{
			public static int Value;
			static MyClass()
			{
				Console.WriteLine(typeof(MyClass<T>));
			}
		}

		class GenericOpenAndClosedTypesMainClass
		{
			static void GenericOpenAndClosedTypesMain()
			{
				MyClass<MyClass<string>>.Value = 10;
				Console.WriteLine(typeof(MyClass<>)); // prints MyClass`1[T]
			}
		}
	}

	// *** 8 *** Generic Type Inference
	public class GenericTypeInference
	{
		class GenericTypeInferenceMainClass
		{
			class MyClass<T>
			{
				public MyClass(T arg) { }
			}
			static void P<T>(T item) { Console.WriteLine(item); }
			static void GenericTypeInferenceMain()
			{
				P(5);
				P("Yianni"); // = P<string>("Yianni") - type inference - not having to state/pass the argument type, only works with generic methods
				P("yanni");

				//MyClass m = new MyClass(5); // type inference does not work with generic class types, only with methods
				MyClass<int> m = new MyClass<int>(5);
			}
		}
	}

	// *** 9 *** Optimizing Code Bloat Out - compiler has to create code for each value type, but will reuse code for reference types
	public class OptimizingCodeBloatOut
	{
		class OptimizingCodeBloatOutMainClass
		{
			//static void DoSomething(int arg) { }
			//static void DoSomething(char arg) { }
			//static void DoSomething(bool arg) { }
			//static void DoSomething(OptimizingCodeBloatOutMainClass arg) { }
			//static void DoSomething(string arg) { }
			static void DoSomething<T>(T arg) { }  // compiler has to copy and paste this method over and over for each value type, instantiates it for every type argument we give it
			static void OptimizingCodeBloatOutMain()
			{
				DoSomething(5);
				DoSomething('j');
				DoSomething(false);
				DoSomething(new OptimizingCodeBloatOutMainClass()); // compiler will reuse code for reference types, because references (the pointer to the object on the heap) are the same size
				DoSomething("Hello Dolly");
				DoSomething(new object());
			}
		}
	}

	// *** 10 *** Generic Constraints Part 1
	public class GenericConstraintsPart1
	{
		class ClassOne { public ClassOne() { } }
		class ClassTwo : ClassOne { public ClassTwo() { } }
		class ClassThree { public ClassThree() { } }

		class GenericConstraintsPart1MainClass
		{
			//static T ProduceA<T>() where T : new() // require that T has a parameterless constructor
			static T ProduceA<T>() where T : ClassOne, new() // constraint on our generic type that requires T is/inherits ClassOne and has a parameterless constructor
			{
				T returnVal = new T(); // we are not guaranteed about the number and type of parameters to generic T's constructor
				return returnVal;
			}
			static void GenericConstraintsPart1Main()
			{
				ProduceA<ClassOne>();
				ProduceA<ClassTwo>();
				//ProduceA<ClassThree>(); // does not work because ProduceA requires a type that is/inherits ClassOne
			}
		}
	}

	// *** 11 *** Generic Constraints Part 2

	// *** 12 *** Generic Constraints Part 3

	// *** 13 *** Generic Constraints - Some Good Uses
	public class GenericConstraintsSomeGoodUses
	{
		class GenericConstraintsSomeGoodUsesMainClass
		{
			static void TakeA<T>(T arg) where T : IComparable { } // can avoid boxing when T is a value type
			static void TakeA(IComparable arg) { } // boxing occurs with int 5 below assigned to this 'arg'
			static void GenericConstraintsSomeGoodUsesMain()
			{
				TakeA<int>(5);
				TakeA(5); // boxing occurs
			}
		}
	}

	#endregion GENERICS

	#region LINQ

	// *** 1 *** Extension Methods Intro - go between between regular instance methods and adding some functionality to a type; extension methods need to be static and in a static class
	static class ExtensionsMethodIntroMainClass
	{
		static DateTime Combine(this DateTime datePart, DateTime timePart)
		{
			return new DateTime(datePart.Year, datePart.Month, datePart.Day, timePart.Hour, timePart.Minute, timePart.Second);
		}
		static void ExtensionsMethodIntroMain()
		{
			// structs (DateTime) are implicitly sealed (cannot be inherited from)
			DateTime date = DateTime.Parse("1/5/2025");
			DateTime time = DateTime.Parse("1/1/0001 9:55pm");
			DateTime combined1 = Combine(date, time);
			DateTime combined2 = date.Combine(time);
		}
	}

	// *** 2 *** Why The 'this' On Extension Methods    
    static class MyExtensionHelpers
	{
		public static DateTime CombineDate(this DateTime datePart, DateTime timePart)
		{
			return new DateTime(datePart.Year, datePart.Month, datePart.Day, timePart.Hour, timePart.Minute, timePart.Second);
		}
	}
	class Cow
	{
		public int numMoos;
	}
	static class CowMethods
	{
		public static void Moo(this Cow _this)
		{
			_this.numMoos++;
			Console.WriteLine("Moooo " + _this.numMoos);
		}
	}
	static class WhyThisOnExtensionMethodsMainClass
	{	
		static void WhyThisOnExtensionMethodsMain()
		{
			DateTime date = DateTime.Parse("1/5/2025");
			DateTime time = DateTime.Parse("1/1/0001 9:55pm");
			DateTime combined1 = MyExtensionHelpers.CombineDate(date, time);
			DateTime combined2 = date.CombineDate(time);

			Cow c = new Cow();
			Cow c2 = new Cow();
			
			//Cow.Moo(c2);
			c2.Moo();
			//Cow.Moo(c); Cow.Moo(c); Cow.Moo(c);
			c.Moo(); c.Moo(); c.Moo();
		}
	}

	// *** 3 *** Extension Methods Caveats - normal static methods that cannot access protected or private data members of the type we're extending

	// *** 4 *** Hello LINQ
	class HelloLinqMainClass
	{
		static void HelloLinqMain()
		{
			int[] numbers = new[] { 2, 4, 8, 1, 9, 2, 0, 3, 4, 2 };
			var result =
				from n in numbers
				where n < 5
				select n;

			char[] chars = new[] { 'c', 'p', 'o', 't', 'i' };
			var vowels =
				from letter in chars
				where letter == 'a' || letter == 'e' || letter == 'i' || letter == 'o' || letter == 'u'
				select letter;

			foreach(char c in vowels)
				Console.WriteLine(c);
		}
	}

	// *** 5 *** Basic LINQ Translation
	class BasicLinqTranslationMainClass
	{
		static void BasicLinqTranslationMain()
		{
			int[] numbers = new[] { 2, 4, 9, 4, 2 };
			// free form comprehensive syntax; query expression syntax
			var result1 = 
				from n in numbers
				where n < 5
				select n;
			// intermediate extension method syntax
			var result2 = 
				numbers
					.Where(n => n < 5) // our predicate
					.Select(n => n);
			// static method call syntax
			var result3 = 
					Enumerable.Select(
						Enumerable.Where(numbers, n => n < 5),
						n => n);
		}
	}

	// *** 6 *** Writing Your Own LINQ Extension Method
	static class WritingYourOwnLinqExtensionMethodMainClass
	{
		static IEnumerable<int> Where(this int[] ints, Func<int, bool> gauntlet)
		{
			Console.WriteLine("My Where()");
			foreach (int i in ints)
				if (gauntlet(i))
					yield return i;
		}

		static void WritingYourOwnLinqExtensionMethodMain()
		{
			int[] numbers = new[] { 2, 4, 9, 4, 13, 2 };
			var result1 =
				from n in numbers
				where n < 5
				select n;
			var result2 =
				numbers
					.Where(n => n < 5)
					.Select(n => n);
			var result3 =
					Enumerable.Select(
						Enumerable.Where(numbers, n => n < 5),
						n => n);
			foreach (int worthyInt in result3) // does not call our IEnumerable<int>.Where method as result3 explicitly uses Enumerable.Where
				Console.WriteLine(worthyInt);
			foreach (int worthyInt in result2) // calls our IEnumerable<int>.Where method
				Console.WriteLine(worthyInt);
		}
	}

	// *** 7 *** LINQ Degenerate Select Clauses - compiler will not waste run time on select clause
	static class LinqDegenerateSelectClausesMainClass
	{
		static IEnumerable<int> Where(this int[] ints, Func<int, bool> gauntlet)
		{
			Console.WriteLine("My Where()");
			foreach (int i in ints)
				if (gauntlet(i))
					yield return i;
		}

		static void LinqDegenerateSelectClausesMain()
		{
			int[] numbers = new[] { 2, 4, 9, 4, 13, 2 };
			var result1 =
				from n in numbers
				where n < 5
				select n;
			var result2 =
				numbers
					.Where(n => n < 5);
					//.Select(n => n); // compiler deletes this statement - will not waste run time on degenerate Select() clause
			foreach (int worthyInt in result1)
				Console.WriteLine(worthyInt);
			foreach (int worthyInt in result2)
				Console.WriteLine(worthyInt);
		}
	}

	// *** 8 *** LINQ - Compiler Translation of a Larger Example
	static class LinqCompilerTranslationLargerExampleMainClass
	{
		static void LinqCompilerTranslationLargerExampleMain()
		{
			int[] numbers = new[] { 2, 4, -2, 9, 4, 13, 2 };
			var result1 =
				from n in numbers
				orderby 8
				where n < 5
				where 0 < n
				where new Random().Next() == 23
				orderby n
				orderby n * 2 + 3
				orderby 5
				select n;

			var result2 =
				numbers
				.OrderBy(n => 8)
				.Where(n => n < 5)
				.Where(n => 0 < n)
				.Where(n => new Random().Next() == 23)
				.OrderBy(n => n)
				.OrderBy(n => n * 2 + 3)
				.OrderBy(n => 5);

			var result3 =
				Enumerable.OrderBy(
					Enumerable.OrderBy(
						Enumerable.OrderBy(
							Enumerable.Where(
								Enumerable.Where(
									Enumerable.Where(
										Enumerable.OrderBy(numbers, n => 8), 
										n => n < 5), 
									n => 0 < n), 
								n => new Random().Next() == 23), 
							n => n), 
						n => n * 2 + 3), 
					n => 5);
		}
	}

	// *** 9 *** LINQ - Introduction to Deferred Execution - we don't execute until we start to consume
	static class LinqIntroductionDeferredExecutionMainClass
	{
		static IEnumerable<T> Where<T>(this IEnumerable<T> items, Func<T, bool> gauntlet)
		{
			Console.WriteLine("Where");
			foreach (T item in items)
				if (gauntlet(item))
					yield return item;
		}
		static IEnumerable<R> Select<T, R>(this IEnumerable<T> items, Func<T, R> transform)
		{
			Console.WriteLine("Select");
			foreach (T item in items)
					yield return transform(item);
		}
		static void LinqIntroductionDeferredExecutionMain()
		{
			int[] stuff = { 4, 8, 1, 9 };
			IEnumerable<int> result =
				from i in stuff
				where i < 5
				select i + 6;
			IEnumerator<int> rator = result.GetEnumerator();
			while(rator.MoveNext())
				Console.WriteLine(rator.Current);
		}
	}

	// *** 10 *** LINQ - Deferred Execution Explained
	static class LinqDeferredExecutionExplainedMainClass
	{
		static IEnumerable<T> Where<T>(this IEnumerable<T> items, Func<T, bool> gauntlet)
		{
			Console.WriteLine("Where");
			foreach (T item in items)
				if (gauntlet(item))
					yield return item;
		}
		static IEnumerable<R> Select<T, R>(this IEnumerable<T> items, Func<T, R> transform)
		{
			Console.WriteLine("Select");
			foreach (T item in items)
				yield return transform(item);
		}
		static void LinqDeferredExecutionExplainedMain()
		{
			int[] stuff = { 4, 8, 1, 9 };
			IEnumerable<int> result2 =
				stuff
					.Where(i => i < 5)
					.Select(i => i + 6);
			IEnumerable<int> result3 =
				Select(
					Where(stuff, i => i < 5),
					i => i + 6);

			IEnumerator<int> rator = result2.GetEnumerator();
			while (rator.MoveNext())
				Console.WriteLine(rator.Current);
		}
	}

	// *** 11 *** LINQ - Deferred Execution - Assembly Line
	static class LinqDeferredExecutionAssemblyLineMainClass
	{
		static IEnumerable<T> Where<T>(this IEnumerable<T> items, Func<T, bool> gauntlet)
		{
			Console.WriteLine("Where");
			foreach (T item in items)
				if (gauntlet(item))
					yield return item;
		}
		static IEnumerable<R> Select<T, R>(this IEnumerable<T> items, Func<T, R> transform)
		{
			Console.WriteLine("Select");
			foreach (T item in items)
				yield return transform(item);
		}
		static void LinqDeferredExecutionAssemblyLineMain()
		{
			int[] stuff = { 4, 13, 8, 1, 9 };
			IEnumerable<string> result = 
				stuff
				.Where(i => i < 10).Where(i => 4 < i).Select(i => i * 3)
				.Where(i => i % 2 == 0).Select(i => i + "Yianni");
			IEnumerator<string> rator = result.GetEnumerator();
			while (rator.MoveNext())
				Console.WriteLine(rator.Current);
		}
	}

	// *** 12 *** LINQ - Deferred Execution - Runtime Created Assembly Line
	class LinqDeferredExecutionRuntimeCreatedAssemblyLineMainClass
	{
		static void LinqDeferredExecutionRuntimeCreatedAssemblyLineMain()
		{
			int[] stuff = { 4, 13, 8, 1, 9 };
			/*IEnumerable<int> result1 = stuff;
			for (int j = 0; j < 3; j++)
			{
				if (RandomBool)
					result1 = result1.Where(i => i < 8);
				if (RandomBool)
					result1 = result1.Where(i => i > 2);
				if (RandomBool)
					result1 = result1.Select(i => i * 2);
				if (RandomBool)
					result1 = result1.Select(i => i + 9);
			}*/
			IEnumerable<int> result1 = stuff.Where(i => i < 8);
			var result2 = result1.Where(i => i > 2).Select(i => i * 2 + 9);
			var result3 = result1.OrderBy(i => i).Select(i => i % 7).Where(i => i < 20);
		}
		//static Random rand = new Random();
		//static bool RandomBool { get { return rand.Next() % 2 == 0; } }
	}

	public static class LinqCustomers
	{
		class Customers : IEnumerable<Customer>
		{
			Customer[] items;
			public int Count { get; set; }
			public Customers(int capacity = 10)
			{
				items = new Customer[capacity];
			}
			public void Add(Customer item)
			{
				EnsureCapacity();
				items[Count++] = item;
			}
			void EnsureCapacity()
			{
				EnsureCapacity(Count + 1);
			}
			void EnsureCapacity(int neededCapacity)
			{
				if (neededCapacity > items.Length)
				{
					int targetSize = items.Length * 2;
					if (targetSize < neededCapacity)
						targetSize = neededCapacity;
					Array.Resize(ref items, targetSize);
				}
			}
			IEnumerator<Customer> IEnumerable<Customer>.GetEnumerator()
			{
				//throw new NotImplementedException();
				for (int i = 0; i < Count; i++)
					yield return items[i];
			}
			IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		}
		class Customer
		{
			public string CustomerID { get; set; }
			public string ContactName { get; set; }
			public string CompanyName { get; set; }
			public string Country { get; set; }
			public string City { get; set; }
			public List<Order> Orders { get; set; }
		}
		class Orders : IEnumerable<Order>
		{
			Order[] items;
			public int Count { get; set; }
			public Orders(int capacity = 10)
			{
				items = new Order[capacity];
			}
			public void Add(Order item)
			{
				EnsureCapacity();
				items[Count++] = item;
			}
			void EnsureCapacity()
			{
				EnsureCapacity(Count + 1);
			}
			void EnsureCapacity(int neededCapacity)
			{
				if (neededCapacity > items.Length)
				{
					int targetSize = items.Length * 2;
					if (targetSize < neededCapacity)
						targetSize = neededCapacity;
					Array.Resize(ref items, targetSize);
				}
			}
			IEnumerator<Order> IEnumerable<Order>.GetEnumerator()
			{
				//throw new NotImplementedException();
				for (int i = 0; i < Count; i++)
					yield return items[i];
			}
			IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
		}
		class Order
		{
			public int OrderID { get; set; }
			public string CustomerID { get; set; }
			public DateTime OrderDate { get; set; }
			public string ShipCountry { get; set; }
			public Customer Customer { get; set; }
		}

		// *** 13 *** LINQ - More Realistic Data (Northwind Database)
		static class LinqMoreRealisticDataMainClass
		{
			static void LinqMoreRealisticDataMain()
			{
				//IEnumerable<Customer> customers = new Customers();
				var customers = new Customers();
				customers.Add(new Customer() { ContactName = "Bob", CompanyName = "Robert", Country = "USA" });
				customers.Add(new Customer() { ContactName = "Joe", CompanyName = "Joseph", Country = "USA" });
				customers.Add(new Customer() { ContactName = "Jim", CompanyName = "James", Country = "Colombia" });
				customers.Add(new Customer() { ContactName = "Yoshi", CompanyName = "Yoshi San", Country = "Japan" });

				foreach (Customer c in customers.Take(5))
				{
					Console.WriteLine(c.ContactName);
					Console.WriteLine(c.CompanyName);
					Console.WriteLine(c.Country);
					Console.WriteLine();
				}
			}
		}

		// *** 14 *** LINQ - Projections
		static class LinqProjectionsMainClass
		{
			static void LinqProjectionsMain()
			{
				var result =
					from c in new Customers()
					select new { c.ContactName, c.CompanyName };
				foreach (var row in result)
				{
					Console.WriteLine(row.ContactName);
					Console.WriteLine(row.CompanyName);
				}
				var result2 =
					new Customers().Select(c => new { c.ContactName, c.CompanyName });
				var result3 =
					Enumerable.Select(new Customers(), c => new { c.ContactName, c.CompanyName });
				var result4 =
					Enumerable.Select(new Customers(), AnonymousFunc);
				foreach (var row in result4)
					Console.WriteLine(row.ContactName);
			}
			static AnonymousJunk AnonymousFunc(Customer c)
			{
				return new AnonymousJunk(c.ContactName, c.CompanyName);
			}
			class AnonymousJunk
			{
				public string ContactName { get; private set; }
				public string CompanyName { get; private set; }
				public AnonymousJunk(string contactName, string companyName)
				{
					ContactName = contactName;
					CompanyName = companyName;
				}
			}
		}

		// *** 15 *** LINQ - order by
		class LinqOrderByMainClass
		{
			static void LinqOrderByMain()
			{
				var customers = new Customers();
				var result =
					from c in customers
					orderby c.Country descending, c.ContactName descending
					select c;
				var result2 =
					customers
						.OrderByDescending(c => c.Country)
						.ThenBy(c => c.ContactName);
						//.OrderBy(c => c.ContactName); // does not preserve previous OrderByDescending

				foreach (var c in result2)
					Console.WriteLine(c.Country + " " + c.ContactName);
			}
		}

		// *** 16 *** LINQ - Random Ordering
		class LinqRandomOrderingMainClass
		{
			static void LinqRandomOrderingMain()
			{
				var rand = new Random();
				var customers = new Customers();
				var contactNames =
					//customers.OrderBy(c => c.ContactName).Select(c => c.ContactName);
					customers.OrderBy(c => rand.Next()).Select(c => c.ContactName).Take(5); // each customer gets a random number, and is ordered by that random number
				foreach (string name in contactNames)
					Console.WriteLine(name);
			}
		}

		// *** 17 *** LINQ - group by - IGrouping is an IEnumerable with a Key
		class LinqGroupByMainClass
		{
			static void LinqGroupByMain()
			{
				var customers = new Customers();
				foreach(Customer c in customers.OrderBy(c => c.Country))
					Console.WriteLine(c.Country + " " + c.ContactName);

				// result is an IEnumerable of groups
				var result =
					from c in customers
					group c by c.Country;
				var result2 =
					customers.GroupBy(c => c.Country);
				foreach(IGrouping<string, Customer> group in result)
				{
					Console.WriteLine(group.Key + ":");
					foreach (Customer c in group)
						Console.WriteLine("  " + c.ContactName);
					Console.WriteLine();
				}
			}
		}

		// *** 18 *** LINQ - Counting Elements In Groups
		class LinqCountingElementsInGroupsMainClass
		{
			static void LinqCountingElementsInGroupsMain()
			{
				var customers = new Customers();

				var result2 =
					customers.GroupBy(c => c.Country);
				var largestGroupsFirst =
					from g in result2
					orderby g.Count() descending
					select new { Country = g.Key, NumCustomers = g.Count() };
				foreach(var result in largestGroupsFirst)
					Console.WriteLine(result);
			}
		}

		// *** 19 *** LINQ - let Clauses and Also Transparent Identifiers - use let to define variables to eliminate redundant calculations (g.Count() calculated twice in video 18)
		class LinqLetClausesTransparentIdentifiersMainClass
		{
			static void LinqLetClausesTransparentIdentifiersMain()
			{
				var customers = new Customers();

				var result2 =
					customers.GroupBy(c => c.Country);
				var largestGroupsFirst =
					from g in result2
					let NumCustomers = g.Count()
					orderby NumCustomers descending
					select new { Country = g.Key, NumCustomers };

				var largestGroupsFirst2 =
					result2
					.Select(g => new { g, NumCustomers = g.Count() }) // transparent identifier 'new { g, NumCustomers = g.Count() }' - temporary anonymous type
					.OrderByDescending(at => at.NumCustomers)
					.Select(at => new { Country = at.g.Key, at.NumCustomers });

				foreach (var result in largestGroupsFirst2)
					Console.WriteLine(result);
			}
		}

		// *** 20 *** LINQ - Introduction to into - compiler takes the above query and turns it into the source for the query below it
		class LinqIntroductionToIntoMainClass
		{
			static void LinqIntroductionToIntoMain()
			{
				var customers = new Customers();

				var largestGroupsFirst =
					from c in customers
					group c by c.Country into g // rolls forward and allows us to query
					let NumCustomers = g.Count()
					orderby NumCustomers descending
					select new { Country = g.Key, NumCustomers };

				foreach (var result in largestGroupsFirst)
					Console.WriteLine(result);
			}
		}

		// *** 21 *** LINQ - into Translation
		class LinqIntoTranslationMainClass
		{
			static void LinqIntoTranslationMain()
			{
				var customers = new Customers();

				var largestGroupsFirst =
					from g in
						from c in customers
						group c by c.Country
					let NumCustomers = g.Count()
					orderby NumCustomers descending
					select new { Country = g.Key, NumCustomers };

				var largestGroupsFirst2 =
					customers
					.GroupBy(c => c.Country)
					.Select(g => new { g, NumCustomers = g.Count() })
					.OrderByDescending(tp => tp.NumCustomers)
					.Select(tp => new { Country = tp.g.Key, tp.NumCustomers });

				var largestGroupsFirst3 =
					Enumerable.Select(
						Enumerable.OrderByDescending(
							Enumerable.Select(
								Enumerable.GroupBy(customers, c => c.Country), 
								g => new { g, NumCustomers = g.Count() }), 
							tp => tp.NumCustomers), 
						tp => new { Country = tp.g.Key, tp.NumCustomers });

				foreach (var result in largestGroupsFirst)
					Console.WriteLine(result);
			}
		}

		// *** 22 *** LINQ - Grouping By Multiple Fields
		class LinqGroupingByMultipleFieldsMainClass
		{
			static void LinqGroupingByMultipleFieldsMain()
			{
				var customers = new Customers();

				var result =
					from c in customers
					orderby c.Country, c.City
					group c by new { c.Country, c.City };
				foreach(var cityCountryGroup in result)
				{
					Console.WriteLine(cityCountryGroup.Key + ":");
					foreach(Customer c in cityCountryGroup)
						Console.WriteLine("   " + c.ContactName);
				}
			}
		}

		// *** 23 *** LINQ - Selecting (Projecting) While Grouping
		class LinqSelectingProjectingWhileGroupingMainClass
		{
			static void LinqSelectingProjectingWhileGroupingMain()
			{
				var customers = new Customers();

				var result =
					from c in customers
					group c.ContactName by new { c.Country, c.City };
				var result2 =
					customers
						.GroupBy(c => new { c.Country, c.City }, c => c.ContactName);
				foreach(var group in result2)
				{
					Console.WriteLine(group.Key + ":");
					foreach(string contactName in group)
						Console.WriteLine("   " + contactName);
				}
			}
		}

		// *** 24 *** LINQ - Let Clauses and Even Deeper Transparent Identifiers
		class LinqLetClausesDeeperTransparentIdentifiersMainClass
		{
			static void LinqLetClausesDeeperTransparentIdentifiersMain()
			{
				var inputs = new []
				{
					new { a = 1, b = 2, c = 3 },
					new { a = 2, b = 9, c = 4 },
					new { a = 7, b = 3, c = 6 }
				};
				var result =
					from coef in inputs
					let negB = -coef.b
					let discriminant = coef.b * coef.b - 4 * coef.a * coef.c
					let twoA = 2 * coef.a
					select new 
					{
						FirstRoot = (negB + discriminant) / twoA,
						SecondRoot = (negB - discriminant) / twoA
					};
				var result2 =
					inputs
					.Select(coef => new { coef, negB = -coef.b })
					.Select(tp1 => new { tp1, discriminant = tp1.coef.b * tp1.coef.b - 4 * tp1.coef.a * tp1.coef.c })
					.Select(tp2 => new { tp2, twoA = 2 * tp2.tp1.coef.a })
					.Select(tp3 => new 
					{
						FirstRoot =  (tp3.tp2.tp1.negB + tp3.tp2.discriminant) / tp3.twoA,
						SecondRoot = (tp3.tp2.tp1.negB - tp3.tp2.discriminant) / tp3.twoA
					});
			}
		}

		// *** 25 *** LINQ Joins
		class LinqJoinsMainClass
		{
			static void LinqJoinsMain()
			{
				var customers = new Customers();
				var orders = new Orders();

				var results =
					from c in customers
					join o in orders
						on c.CustomerID equals o.CustomerID
					select new {c.ContactName, o.OrderDate };
				foreach(var pair in results)
					Console.WriteLine(pair.ContactName + " " + pair.OrderDate);

				var results2 =
					customers.Join(orders, c => c.CustomerID, o => o.CustomerID,
						(c, o) => new { c.ContactName, o.OrderDate });
				foreach (var pair in results2)
					Console.WriteLine(pair.ContactName + " " + pair.OrderDate);

				var results3 =
					Enumerable.Join(customers, orders, c => c.CustomerID, o => o.CustomerID,
						(c, o) => new { c.ContactName, o.OrderDate });
				foreach (var pair in results3)
					Console.WriteLine(pair.ContactName + " " + pair.OrderDate);
			}
		}

		// *** 26 *** LINQ Join vs Navigation Property
		class LinqJoinVsNavigationPropertyMainClass
		{
			static void LinqJoinVsNavigationPropertyMain()
			{
				var customers = new Customers();
				var orders = new Orders();

				var results =
					from c in customers
					join o in orders
						on c.CustomerID equals o.CustomerID
					select new { c.ContactName, o.OrderDate };
				foreach (var pair in results)
					Console.WriteLine(pair.ContactName + " " + pair.OrderDate);
				Console.WriteLine();
				var maria = customers.First();
				Console.WriteLine(maria.ContactName);
				foreach(Order o in maria.Orders) // maria.Orders is a navigational property
					Console.WriteLine("\t" + o.OrderDate);
			}
		}

		// *** 27 ***  Join Clause Translation With Other Clauses Besides Select
		class LinqJoinClauseTranslationMainClass
		{
			static void LinqJoinClauseTranslationMain()
			{
				var customers = new Customers();
				var orders = new Orders();

				var results1 =
					from c in customers
					join o in orders
						on c.CustomerID equals o.CustomerID
					orderby c.ContactName // compiler has to take result of join and pass it from join into the orderby, and then the orderby sends it on down through the select
					select new { c.ContactName, o.OrderDate };
				foreach (var pair in results1)
					Console.WriteLine(pair.ContactName + " " + pair.OrderDate);

				var results2 =
					customers.Join(orders, c => c.CustomerID, o => o.CustomerID,
						(c, o) => new { c, o }).
						OrderBy(tp => tp.c.ContactName). // if it can't slam the select in with the join call, then it has to use the transparent identifier to send it on through
						Select(tp => new { tp.c.ContactName, tp.o.OrderDate });
				;
				foreach (var pair in results2)
					Console.WriteLine(pair.ContactName + " " + pair.OrderDate);
			}
		}

		// *** 28 *** LINQ Join and Group
		class LinqJoinAndGroupMainClass
		{
			static void LinqJoinAndGroupMain()
			{
				var customers = new Customers();
				var orders = new Orders();

				// get customers grouped by orders
				var results =
					from c in customers
					join o in orders
						on c.CustomerID equals o.CustomerID
					group o by c into g
					let NumOrders = g.Count()
					orderby NumOrders descending
					select new { g.Key.ContactName, NumOrders };

				foreach (var pair in results)
					Console.WriteLine(pair.ContactName + ": " + pair.NumOrders);
			}
		}

		// *** 29 *** LINQ Join into Group - if you have a join followed by an into, then the join will also group
		class LinqJoinIntoGroupMainClass
		{
			static void LinqJoinIntoGroupMain()
			{
				var customers = new Customers();
				var orders = new Orders();

				var results =
					from c in customers
					join o in orders
						on c.CustomerID equals o.CustomerID into thisCustomersOrders // no longer an IGrouping, thisCustomersOrders is an IEnumerable, all Orders are packed in this IEnumerable
					let NumOrders = thisCustomersOrders.Count()
					orderby NumOrders descending
					select new { c.ContactName, NumOrders };

				foreach (var pair in results)
					Console.WriteLine(pair.ContactName + ": " + pair.NumOrders);
			}
		}

		// *** 30 *** LINQ Group Join Example - Join into Group (Group while Join) more efficient than Join and Group (Join then Group)
		class LinqGroupJoinExampleMainClass
		{
			static void LinqGroupJoinExampleMain()
			{
				var customers = new Customers();
				var orders = new Orders();

				var results =
					from c in customers
					join o in orders
						on c.CustomerID equals o.CustomerID
					group o by c into g
					let NumOrders = g.Count()
					orderby NumOrders descending
					select new { g.Key.ContactName, NumOrders };

				var resultsTranslated =
					customers
					.Join(orders, c => c.CustomerID, o => o.CustomerID,
						(c, o) => new { c, o })
					.GroupBy(tp => tp.c, tp => tp.o)
					.Select(g => new { g, NumOrders = g.Count() })
					.OrderByDescending(tp2 => tp2.NumOrders)
					.Select(tp2 => new { tp2.g.Key.ContactName, tp2.NumOrders });

				foreach (var pair in results)
					Console.WriteLine(pair.ContactName + ": " + pair.NumOrders);

				var results2 =
					from c in customers
					join o in orders
						on c.CustomerID equals o.CustomerID into thisCustomersOrders
					let NumOrders = thisCustomersOrders.Count()
					orderby NumOrders descending
					select new { c.ContactName, NumOrders };

				var results2Translated =
					customers
					.GroupJoin(orders, c => c.CustomerID, o => o.CustomerID,
						(c, os) => new { c, thisCustomersOrders = os })
					.Select(tp => new { tp, NumOrders = tp.thisCustomersOrders.Count() })
					.OrderByDescending(tp2 => tp2.NumOrders)
					.Select(tp2 => new { tp2.tp.c.ContactName, tp2.NumOrders });
			}
		}

		// *** 31 *** LINQ Outer Join - inner join looks at intersection, outer join gets everything
		class LinqOuterJoinMainClass { static void LinqOuterJoinMain() { } }

		// *** 32 *** LINQ Join into Where
		class LinqJoinIntoWhereMainClass
		{
			static void LinqJoinIntoWhereMain()
			{
				var customers = new Customers();
				var orders = new Orders();

				IEnumerable<string> customersWithoutOrders =
					from c in customers
					join o in orders
						on c.CustomerID equals o.CustomerID into thisCustomersOrders
					where thisCustomersOrders.Count() == 0
					select c.ContactName;
				foreach (string name in customersWithoutOrders)
					Console.WriteLine(name);
				Console.WriteLine();

				IEnumerable<string> customersWithoutOrders2 =
					customers
					.GroupJoin(orders, c => c.CustomerID, o => o.CustomerID, 
						(c, os) => new { c, thisCustomersOrders = os })
					.Where(tp => tp.thisCustomersOrders.Count() == 0)
					.Select(tp => tp.c.ContactName);
				foreach (string name in customersWithoutOrders2)
					Console.WriteLine(name);
			}
		}
	}

	// *** Linq Examples ***
	public class LinqExamples
	{
		class Pet
		{
			public string Name { get; set; }
			public double Age { get; set; }
		}
		class Person
		{
			internal int PersonId;
			internal string car;
		}

		public static void LinqExamplesMain()
		{
			// count occurences of n in array
			int[] intArray = { 1, 2, 3, 4, 5, 9, 8, 7, 6, 5 };
			int n = 5; //intArray.Max();
			int count = intArray.Where(i => i.Equals(n)).Count();
			Console.WriteLine("intArray.Where(i => i.Equals(n)).Count() = " + count);
			count = (from i in intArray where i == n select i).Count();
			Console.WriteLine("(from i in intArray where i == n select i).Count() = " + count);

			// what does this do?
			//var z = intArray.Aggregate((x, y) => (x + y) % 2 == 0 ? 1 : 0); // 1
			var z = intArray.Aggregate((x, y) => (x + y)); // 50

			// convert array, eg. double each value
			intArray = ((new List<int>(intArray)).ConvertAll(i => i * 2)).ToArray();

			// read an array from the user separated by spaces
			string[] arr_temp = Console.ReadLine().Split(' ');
			intArray = Array.ConvertAll(arr_temp, Int32.Parse);

			// Transform array to new string array - double each int and convert to string
			arr_temp = intArray.Select(x => (2 * x).ToString()).ToArray();

			// sum of lengths of strings in a List
			List<string> stringList = new List<string> { "88888888", "22", "666666", "333" };
			int lengthSum = stringList.Select(x => x.Length).Sum();  // lengthSum: 19
			Console.WriteLine("stringList.Select(x => x.Length).Sum() = " + lengthSum);
			lengthSum = stringList.Sum(x => x.Length);               // lengthSum: 19
			Console.WriteLine("stringList.Sum(x => x.Length) = " + lengthSum);

			/*** GROUP BY EXAMPLES ***/
			// group each number, then count occurences in each group, convert count to new value (double count), sum up: (4 10s, 3 20s, 2 30s, 1 41, 1 50) 4+3+2+1+1 * 2 = 22
			int[] ar = new int[] { 10, 20, 20, 10, 10, 30, 50, 10, 20, 30, 41 };
			int groupCountConvertSum = new List<int>(ar.GroupBy(i => i).Select(i => i.Count())).ConvertAll(x => x * 2).Sum();
			Console.WriteLine("new List<int>(ar.GroupBy(i => i).Select(i => i.Count())).ConvertAll(x => x * 2).Sum() = " + groupCountConvertSum);
			Console.WriteLine("--------------------");
			var bin = new List<int>(ar.GroupBy(i => i).Select(i => i.Count())).ConvertAll(x => x * 2); // bin is { 4*2, 3*2, 2*2, 1*2, 1*2 } = { 8, 6, 4, 2, 2 }
			foreach (int b in bin)
				Console.Write($"{b} ");
			Console.WriteLine("\n********************");

			// Group array elements
			var arGroups =
				from x in ar
				//group x by (x % 2 == 0) into arGroup // this will group by even and odd
				group x by x into arGroup // this will group by element values
				orderby arGroup.Key
				select arGroup;
			foreach (var group in arGroups)
			{
				Console.WriteLine($"{group.Count()} {group.Key}'s:");
				foreach (var x in group)
					Console.WriteLine($"\t{x}");
			}
			Console.WriteLine("********************");

			// Group array elements by even (True) and odd (False)
			int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var arrayGroups = array.GroupBy(x => x % 2 == 0);
			foreach (int c in arrayGroups.Select(i => i.Count())) // selects and prints Count of each grouping (count of 5 False's (odd) and count of 4 True's (even))
				Console.WriteLine("Count = " + c);
			foreach (var key in arrayGroups.Select(i => i.Key)) // selects and prints value of each group's Key (False (odds) and True (evens))
				Console.WriteLine("Key = " + key);
			foreach (var group in arrayGroups.Select(i => i)) // selects and prints Count and Key of each grouping (count of 5 False's (odd) and count of 4 True's (even))
				Console.WriteLine($"Count = {group.Count()}, Key = {group.Key}");

			foreach (var group in arrayGroups)
			{
				Console.WriteLine($"{group.Count()} {group.Key}'s:");
				foreach (var value in group)
					Console.Write($"{value} ");
				Console.WriteLine();
			}

			// Group pets by Math.Floor of age. Then project an anonymous type from each group that consists of the key, count of group's elements, and min and max age in the group.
			List<Pet> pets = new List<Pet> {
				new Pet { Name="Barley", Age=8.3 },
				new Pet { Name="Boots", Age=4.9 },
				new Pet { Name="Whiskers", Age=1.5 },
				new Pet { Name="Daisy", Age=4.3 }
			};
			var petAgeGroups = pets.GroupBy(
				pet => Math.Floor(pet.Age),
				pet => pet.Age,
				(baseAge, ages) => new 
				{
					Key = baseAge,
					Count = ages.Count(),
					Min = ages.Min(),
					Max = ages.Max()
				});
			// returns same grouping as above
			var petAgeGroups2 = pets.GroupBy(
				pet => Math.Floor(pet.Age),
				(age, petGroup) => new 
				{
					Key = age,
					Count = petGroup.Count(),
					Min = petGroup.Min(pet => pet.Age),
					Max = petGroup.Max(pet => pet.Age)
				});
			foreach (var petAgeGroup in petAgeGroups)
			{
				Console.WriteLine("\nAge group: " + petAgeGroup.Key);
				Console.WriteLine("Number of pets in this age group: " + petAgeGroup.Count);
				Console.WriteLine("Minimum age: " + petAgeGroup.Min);
				Console.WriteLine("Maximum age: " + petAgeGroup.Max);
			}

			// Group pets by Math.Floor of age
			IEnumerable<IGrouping<int, string>> petAgeGroups3 = pets.GroupBy(pet => (int)Math.Floor(pet.Age), pet => pet.Name); // extension method syntax
			//IEnumerable<IGrouping<int, string>> petAgeGroups3 = from pet in pets group pet.Name by (int)Math.Floor(pet.Age); // query expression syntax
			foreach (IGrouping<int, string> petAgeGroup3 in petAgeGroups3)
			{
				Console.WriteLine(petAgeGroup3.Key);
				foreach (string name in petAgeGroup3)
					Console.WriteLine("  " + name);
			}

			// Group by composite key
			var petGroups =
				from pet in pets
				group pet by new { FirstLetter = pet.Name[0], Age = pet.Age > 5 } into petGroup
				orderby petGroup.Key.FirstLetter
				select petGroup;
			Console.WriteLine("\nGroup and order by a compound key:");
			foreach (var petGroup in petGroups)
			{
				string s = petGroup.Key.Age == true ? "older than" : "younger than";
				Console.WriteLine($"Name starts with {petGroup.Key.FirstLetter} and {s} 5 years");
				foreach (var pet in petGroup)
					Console.WriteLine($"\t{pet.Name}");
			}

			// Group cars by person example
			List<Person> persons = new List<Person>(3);
			persons.Add(new Person { PersonId = 1, car = "Ferrari" });
			persons.Add(new Person { PersonId = 1, car = "BMW" });
			persons.Add(new Person { PersonId = 2, car = "Mercedes" });
			var carGroups =
				from person in persons
				group person.car by person.PersonId into carGroup
				select new { PersonId = carGroup.Key, Cars = carGroup.ToList() };
			// as a non-query expression (extension method syntax):
			var carGroups2 = persons.GroupBy(
				person => person.PersonId,
				person => person.car,
				(key, carGroup) => new { PersonId = key, Cars = carGroup.ToList() });
			// returns same grouping as above (as a List)
			var carGroups3 = persons.GroupBy(
				person => person.PersonId,
				(key, personGroup) => new 
				{
					PersonId = key,
					Cars = personGroup.Select(p => p.car).ToList()
				}
				).ToList();
			foreach (var person in carGroups3)
			{
				Console.WriteLine(person.PersonId);
				foreach (string car in person.Cars)
					Console.WriteLine(car);
			}

			// Alternatively, we could use a 'Lookup':
			ILookup<int, string> carsByPersonId = persons.ToLookup(person => person.PersonId, person => person.car);
			IEnumerable<string> carsForPerson = carsByPersonId[1];

			// Read a file skipping the header line and ignoring any empty lines
			IEnumerable<string> lines = File.ReadAllLines("file.txt").Skip(1).Where(line => line.Length > 1);

			int[] arr = new int[] { 3, 3, 2, 7, 1, 4, 2, 3, 6, 3 };
			// Get element with max occurances in an arry
			int maxOccur = arr.GroupBy(i => i).OrderByDescending(g => g.Count()).Select(g => g.Key).First();
			// Get count of element that occurs most in an array
			int maxCount = arr.GroupBy(i => i).OrderByDescending(g => g.Count()).Select(g => g.Count()).First();
		}
	}

	#endregion LINQ

	#region THREADING

	// *** 1 *** Hello World Thread
	public class HelloWorldThread
	{
		public class HelloWorldThreadMainClass
		{
			static void HelloWorldThreadMain()
			{
				var thread = new Thread(DifferentMethod); // takes a delegate
				thread.Start();
			}
			static void DifferentMethod()
			{
				Console.WriteLine("My Name");
			}
		}
	}

	// *** 2 *** Multiple Threads
	public class MultipleThreads
	{
		public class MultipleThreadsMainClass
		{
			static void MultipleThreadsMain()
			{
				for (int i = 0; i < Environment.ProcessorCount; i++)
				{
					var thread = new Thread(DifferentMethod);
					thread.Start(i);
				}
			}
			static void DifferentMethod(object threadID)
			{
				while (true)
					Console.WriteLine("Hello from DifferentMethod(): " + threadID);
			}
		}
	}

	// *** 3 *** Difference Between Background and Foreground Thread - OS process will not terminate until all foreground threads have terminated
	public class BackgroundForegroundThread
	{
		public class BackgroundForegroundThreadMainClass
		{
			static void BackgroundForegroundThreadMain()
			{
				Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
				for (int i = 0; i < Environment.ProcessorCount; i++) // foreground (important) threads
				{
					var thread = new Thread(DifferentMethod);
					thread.IsBackground = true;
					thread.Start(i);
				}
				Console.WriteLine("Leaving Main()");
			}
			static void DifferentMethod(object threadID)
			{
				while (true)
					Console.WriteLine("Hello from DifferentMethod(): " + Thread.CurrentThread.ManagedThreadId); // managed, CLR threads run on top of Windows threads
			}
		}
	}

	// *** 4 *** Thread Synchronization Issue
	public class ThreadSynchronizationIssue
	{
		public class ThreadSynchronizationIssueMainClass
		{
			static int count = 0;
			static void ThreadSynchronizationIssueMain()
			{
				var thread1 = new Thread(IncrementCount);
				var thread2 = new Thread(IncrementCount);
				thread1.Start();
				Thread.Sleep(500);
				thread2.Start();
			}
			static void IncrementCount()
			{
				while (true)
				{
					int temp = count;
					Thread.Sleep(1000);
					count = temp + 1;
					//count++; // not a single (atomic) operation
					Console.WriteLine("Thread ID " + Thread.CurrentThread.ManagedThreadId + " incremented count to " + count);
					Thread.Sleep(1000);
				}
			}
		}
	}

	// *** 5 *** Basic Thread Synchronization
	public class BasicThreadSynchronization
	{
        public class BasicThreadSynchronizationMainClass
		{
			static int count = 0;
			static object baton = new object(); // in order for thread to get into danger zone (when read or write to shared data), it has to have the baton
			static void BasicThreadSynchronizationMain()
			{
				var thread1 = new Thread(IncrementCount);
				var thread2 = new Thread(IncrementCount);
				thread1.Start();
				Thread.Sleep(500);
				thread2.Start();
			}
			static void IncrementCount()
			{
				while (true)
				{
					lock (baton) // lock danger zone
					{
						int temp = count; // temp is local, so each thread gets its own copy
						Thread.Sleep(1000);
						count = temp + 1;
						Console.WriteLine("Thread ID " + Thread.CurrentThread.ManagedThreadId + " incremented count to " + count);
					}
					Thread.Sleep(1000);
				}
			}
		}
	}

	// *** 6 *** Another Thread Synchronization
	public class AnotherThreadSynchronization
	{
        public class AnotherThreadSynchronizationMainClass
		{
			static object baton = new object();
			static Random rand = new Random();
			static void AnotherThreadSynchronizationMain()
			{
				for (int i = 0; i < 5; i++)
				{
					new Thread(UseRestroomStall).Start();
				}
			}
			static void UseRestroomStall()
			{
				Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " trying to obtain the bathroom stall...");
				lock (baton)
				{
					Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " obtained the lock. Doing my business...");
					Thread.Sleep(rand.Next(2000));
					Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " leaving the stall...");
				}
				Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " is leaving the restaurant.");
			}
		}
	}

	// *** 7 *** lock to Monitor Method Calls - lock is syntactic sugar
	public class LockMonitorMethodCalls
	{
        public class LockMonitorMethodCallsMainClass
		{
			static object baton = new object();
			static void LockMonitorMethodCallsMain()
			{
				//lock (baton)
				bool lockTaken = false;
				Monitor.Enter(baton, ref lockTaken);
				try
				{
					Console.WriteLine("Got the baton");
				}
				finally
				{
					if (lockTaken)
						Monitor.Exit(baton);
				}
			}
		}
	}

	// *** 8 *** Threading - Divide and Conquer Part 1
	public class ThreadingDivideConquerOne
	{
        public class ThreadingDivideConquerOneMainClass
		{
			static byte[] values = new byte[500000000];
			static void GenerateInts()
			{
				var rand = new Random(987);
				for (int i = 0; i < values.Length; i++)
					values[i] = (byte)rand.Next(10);
			}
			static void ThreadingDivideConquerOneMain()
			{
				GenerateInts();
				Console.WriteLine("Summing...");
				Stopwatch watch = new Stopwatch();
				watch.Start();
				long total = 0;
				for (int i = 0; i < values.Length; i++)
					total += values[i];
				watch.Stop();
				Console.WriteLine("Total value is: " + total);
				Console.WriteLine("Time to sum: " + watch.Elapsed);
			}
		}
	}

	// *** 9 *** Threading - Divide and Conquer Part 2
	public class ThreadingDivideConquerTwo
	{
        public class ThreadingDivideConquerTwoMainClass
		{
			static byte[] values = new byte[500000000];
			static long[] portionResults;
			static int portionSize;
			static void GenerateInts()
			{
				var rand = new Random(987);
				for (int i = 0; i < values.Length; i++)
					values[i] = (byte)rand.Next(10);
			}
			static void SumYourPortion(object portionNumber)
			{
				long sum = 0;
				int portionNumberAsInt = (int)portionNumber;
				for (int i = portionNumberAsInt * portionSize; i < portionNumberAsInt * portionSize + portionSize; i++)
					sum += values[i];
				portionResults[portionNumberAsInt] = sum;
			}
			static void ThreadingDivideConquerTwoMain()
			{
				portionResults = new long[Environment.ProcessorCount];
				portionSize = values.Length / Environment.ProcessorCount;
				GenerateInts();
				Console.WriteLine("Summing...");
				Stopwatch watch = new Stopwatch();
				watch.Start();
				long total = 0;
				for (int i = 0; i < values.Length; i++)
					total += values[i];
				watch.Stop();
				Console.WriteLine("Total value is: " + total);
				Console.WriteLine("Time to sum: " + watch.Elapsed);
			}
		}
	}

	// *** 10 *** Threading - Divide and Conquer Part 3
	public class ThreadingDivideConquerThree
	{
        public class ThreadingDivideConquerThreeMainClass
		{
			static byte[] values = new byte[500000000];
			static long[] portionResults;
			static int portionSize;
			static void GenerateInts()
			{
				var rand = new Random(987);
				for (int i = 0; i < values.Length; i++)
					values[i] = (byte)rand.Next(10);
			}
			static void SumYourPortion(object portionNumber)
			{
				long sum = 0;
				int portionNumberAsInt = (int)portionNumber;
				int baseIndex = portionNumberAsInt * portionSize;
				for (int i = baseIndex; i < baseIndex + portionSize; i++)
					sum += values[i];
				portionResults[portionNumberAsInt] = sum;
			}
			static void ThreadingDivideConquerThreeMain()
			{
				// Single Thread
				portionResults = new long[Environment.ProcessorCount];
				portionSize = values.Length / Environment.ProcessorCount;
				GenerateInts();
				Console.WriteLine("Summing...");
				Stopwatch watch = new Stopwatch();
				watch.Start();
				long total1 = 0;
				for (int i = 0; i < values.Length; i++)
					total1 += values[i];
				watch.Stop();
				Console.WriteLine("Total value is: " + total1);
				Console.WriteLine("Time to sum: " + watch.Elapsed);
				Console.WriteLine();

				// Divide and Conquer
				watch.Reset();
				watch.Start();
				Thread[] threads = new Thread[Environment.ProcessorCount];
				for (int i = 0; i < Environment.ProcessorCount; i++)
				{
					threads[i] = new Thread(SumYourPortion);
					threads[i].Start(i);
				}
				for (int i = 0; i < Environment.ProcessorCount; i++)
					threads[i].Join(); // after i've started them all, going through each thread and join them - i want to wait until each has completed its job
				long total2 = 0;
				for (int i = 0; i < Environment.ProcessorCount; i++)
					total2 += portionResults[i];
				watch.Stop();
				Console.WriteLine("Total value is: " + total2);
				Console.WriteLine("Time to sum: " + watch.Elapsed);
			}
		}
	}

	// *** 11 *** Producer Consumer Thread Introduction
	// had we started summing threads before producing numbers we would have issue because we need to produce numbers before summing them
	// producer consumer model - instead of having all data beforehand we can do producer consumer model
	// each thread will consume the data, produce data on the fly
	// we create a producing thread and a bunch of consuming threads

	// *** 12 *** Producer Consumer Thread Implementation Part 1
	public class ProducerConsumerThreadOne
	{
        public class ProducerConsumerThreadOneMainClass
		{
			static Queue<int> numbers = new Queue<int>();
			static Random rand = new Random();
			const int NumThreads = 3;
			static int[] sums = new int[NumThreads];
			// producing thread
			static void ProduceNumbers()
			{
				for (int i = 0; i < 10; i++)
				{
					numbers.Enqueue(rand.Next(10));
					Thread.Sleep(rand.Next(1000));
				}
			}
			// consuming threads
			static void SumNumbers(object threadNumber)
			{
				DateTime startTime = DateTime.Now;
				int mySum = 0;
				while((DateTime.Now - startTime).Seconds < 11)
				{
					if (numbers.Count != 0)
						mySum += numbers.Dequeue();
				}
				sums[(int)threadNumber] = mySum;
			}
			static void ProducerConsumerThreadOneMain() {}
		}
	}

	// *** 13 *** Producer Consumer Thread Implementation Part 2
	public class ProducerConsumerThreadTwo
	{
        public class ProducerConsumerThreadTwoMainClass
		{
			static Queue<int> numbers = new Queue<int>();
			static Random rand = new Random();
			const int NumThreads = 3;
			static int[] sums = new int[NumThreads];
			static void ProduceNumbers()
			{
				for (int i = 0; i < 10; i++)
				{
					int numToEnqueue = rand.Next(10);
					Console.WriteLine("Producing thread adding " + numToEnqueue + " to the queue.");
					numbers.Enqueue(numToEnqueue);
					Thread.Sleep(rand.Next(1000));
				}
			}
			static void SumNumbers(object threadNumber)
			{
				DateTime startTime = DateTime.Now;
				int mySum = 0;
				while ((DateTime.Now - startTime).Seconds < 11)
				{
					if (numbers.Count != 0)
					{
						int numToSum = numbers.Dequeue();
						mySum += numToSum;
						Console.WriteLine("Consuming thread #" + threadNumber + " adding " + numToSum + " to its total sum making " + mySum + " for the thread total.");
					}	
				}
				sums[(int)threadNumber] = mySum;
			}
			static void ProducerConsumerThreadOTwoMain()
			{
				var producingThread = new Thread(ProduceNumbers);
				producingThread.Start();
				Thread[] threads = new Thread[NumThreads];
				for (int i = 0; i < NumThreads; i++)
				{
					threads[i] = new Thread(SumNumbers);
					threads[i].Start(i);
				}
				for (int i = 0; i < NumThreads; i++)
					threads[i].Join();
				int totalSum = 0;
				for (int i = 0; i < NumThreads; i++)
					totalSum += sums[i];
				Console.WriteLine("Done adding. Total is " + totalSum);
			}
		}
	}

	// *** 14 *** Producer Consumer Thread Synchronization Issues
	public class ProducerConsumerThreadSynchronizationIssues
	{
        public class ProducerConsumerThreadSynchronizationIssuesMainClass
		{
			static Queue<int> numbers = new Queue<int>();
			static Random rand = new Random();
			const int NumThreads = 3;
			static int[] sums = new int[NumThreads];
			static void ProduceNumbers()
			{
				for (int i = 0; i < 10; i++)
				{
					int numToEnqueue = rand.Next(10);
					Console.WriteLine("Producing thread adding " + numToEnqueue + " to the queue.");
					numbers.Enqueue(numToEnqueue);
					Thread.Sleep(rand.Next(1000));
				}
			}
			static void SumNumbers(object threadNumber)
			{
				DateTime startTime = DateTime.Now;
				int mySum = 0;
				while ((DateTime.Now - startTime).Seconds < 11)
				{
					if (numbers.Count != 0)
					{
						int numToSum;
						try
						{
							numToSum = numbers.Dequeue();
						}
						catch
						{
							Console.WriteLine("Thread #" + threadNumber + " having an issue!");
							throw;
						}
						mySum += numToSum;
						Console.WriteLine("Consuming thread #" + threadNumber + " adding " + numToSum + " to its total sum making " + mySum + " for the thread total.");
					}
				}
				sums[(int)threadNumber] = mySum;
			}
			static void ProducerConsumerThreadSynchronizationIssuesMain()
			{
				var producingThread = new Thread(ProduceNumbers);
				producingThread.Start();
				Thread[] threads = new Thread[NumThreads];
				for (int i = 0; i < NumThreads; i++)
				{
					threads[i] = new Thread(SumNumbers);
					threads[i].Start(i);
				}
				for (int i = 0; i < NumThreads; i++)
					threads[i].Join();
				int totalSum = 0;
				for (int i = 0; i < NumThreads; i++)
					totalSum += sums[i];
				Console.WriteLine("Done adding. Total is " + totalSum);
			}
		}
	}

	// *** 15 *** Race Condition Example - threads racing to grab an object
	public class RaceCondition
	{
        public class RaceConditionMainClass
		{
			static Queue<int> numbers = new Queue<int>();
			static Random rand = new Random(987);
			const int NumThreads = 3;
			static int[] sums = new int[NumThreads];
			static void ProduceNumbers()
			{
				for (int i = 0; i < 10; i++)
				{
					int numToEnqueue = rand.Next(10);
					Console.WriteLine("Producing thread adding " + numToEnqueue + " to the queue.");
					numbers.Enqueue(numToEnqueue);
					Thread.Sleep(rand.Next(1000));
				}
			}
			static void SumNumbers(object threadNumber)
			{
				DateTime startTime = DateTime.Now;
				int mySum = 0;
				while ((DateTime.Now - startTime).Seconds < 11)
				{
					//lock(numbers)
					{
						if (numbers.Count != 0) // if we remove lock, then all consuming threads will evaluate true and proceed until the first thread reaches the Dequeue()
						{
							int numToSum;
							try
							{
								numToSum = numbers.Dequeue();
							}
							catch
							{
								Console.WriteLine("Thread #" + threadNumber + " having an issue!");
								throw;
							}
							mySum += numToSum;
							Console.WriteLine("Consuming thread #" + threadNumber + " adding " + numToSum + " to its total sum making " + mySum + " for the thread total.");
						}
					}
				}
				sums[(int)threadNumber] = mySum;
			}
			static void RaceConditionMain()
			{
				var producingThread = new Thread(ProduceNumbers);
				producingThread.Start();
				Thread[] threads = new Thread[NumThreads];
				for (int i = 0; i < NumThreads; i++)
				{
					threads[i] = new Thread(SumNumbers);
					threads[i].Start(i);
				}
				for (int i = 0; i < NumThreads; i++)
					threads[i].Join();
				int totalSum = 0;
				for (int i = 0; i < NumThreads; i++)
					totalSum += sums[i];
				Console.WriteLine("Done adding. Total is " + totalSum);
			}
		}
	}

	// *** 16 *** ProducerConsumerLock
	public class ProducerConsumerLock
	{
        public class ProducerConsumerLockMainClass
		{
			static Queue<int> numbers = new Queue<int>();
			static Random rand = new Random(987);
			const int NumThreads = 3;
			static int[] sums = new int[NumThreads];
			static void ProduceNumbers()
			{
				for (int i = 0; i < 10; i++)
				{
					int numToEnqueue = rand.Next(10);
					Console.WriteLine("Producing thread adding " + numToEnqueue + " to the queue.");
					numbers.Enqueue(numToEnqueue);
					Thread.Sleep(rand.Next(1000));
				}
			}
			static void SumNumbers(object threadNumber)
			{
				DateTime startTime = DateTime.Now;
				int mySum = 0;
				while ((DateTime.Now - startTime).Seconds < 11)
				{
					int numToSum = -1;
					lock (numbers) // problem with lock, lock was too big (in example 15 Race Condition Example), danger zone is only references to 'numbers'
					{	
						if (numbers.Count != 0)
							numToSum = numbers.Dequeue();
					}
					if (numToSum != -1)
					{
						mySum += numToSum;
						Console.WriteLine("Consuming thread #" + threadNumber + " adding " + numToSum + " to its total sum making " + mySum + " for the thread total.");
					}
				}
				sums[(int)threadNumber] = mySum;
			}
			static void ProducerConsumerLockMain()
			{
				var producingThread = new Thread(ProduceNumbers);
				producingThread.Start();
				Thread[] threads = new Thread[NumThreads];
				for (int i = 0; i < NumThreads; i++)
				{
					threads[i] = new Thread(SumNumbers);
					threads[i].Start(i);
				}
				for (int i = 0; i < NumThreads; i++)
					threads[i].Join();
				int totalSum = 0;
				for (int i = 0; i < NumThreads; i++)
					totalSum += sums[i];
				Console.WriteLine("Done adding. Total is " + totalSum);
			}
		}
	}

	// *** 17 *** Best  Practices When Lock on Object
	public class LockOnObject
	{
		class BathroomStall
		{
			public void BeUsed(int userNumber)
			{
				for (int i = 0; i < 5; i++)
				{
					Console.WriteLine("Being used by " + userNumber);
					Thread.Sleep(500);
				}
			}
		}
		class LockOnObjectMainClass
		{
			static BathroomStall stall = new BathroomStall();
			static void LockOnObjectMain()
			{
				for (int i = 0; i < 3; i++)
					new Thread(RegularUsers).Start();
				new Thread(TheWeirdGuy).Start(); // will interupt RegularUsers since doesn't use lock
			}
			static void RegularUsers() {
				lock (stall)
					stall.BeUsed(Thread.CurrentThread.ManagedThreadId);
			}
			static void TheWeirdGuy() {
				stall.BeUsed(99);
			}
		}
	}

	// *** 18 *** What Lock Really Means - we don't actually lock on the object, what we are really locking on is CLR resource called synchronization block on objects on heap

	// *** 19 *** Synchronized Containers - Sychronized method - I can pass a queue I've created to synchronize and it will return back a reference to a queue that allows
	// only one thread in it at a time. Basically same thing we are doing with our lock, except now we don't need to say lock ourselves, the queue is gonna worry about it.
	// It will only let one thread execute code inside of it at one time. They didn't add this feature when they added the generic queue.
	public class SynchronizedContainers {

        public class SynchronizedContainersMainClass
		{
			static MySynchronizedQueue<int> numbers = new MySynchronizedQueue<int>();
			static Random rand = new Random(987);
			const int NumThreads = 3;
			static int[] sums = new int[NumThreads];

			static void ProduceNumbers()
			{
				for (int i = 0; i < 10; i++)
				{
					int numToEnqueue = rand.Next(10);
					Console.WriteLine("Producing thread adding " + numToEnqueue + " to the queue.");
					//lock (numbers) - no longer need lock because we have MySynchronizedQueue
					numbers.Enqueue(numToEnqueue);
					Thread.Sleep(rand.Next(1000));
				}
			}

			static void SumNumbers(object threadNumber)
			{
				DateTime startTime = DateTime.Now;
				int mySum = 0;
				while ((DateTime.Now - startTime).Seconds < 11)
				{
					int numToSum = -1;
					lock (numbers.SyncRoot) // we still need this lock
					{
						if (numbers.Count != 0)
						{
							// NO LOCK THAT EXISTS RIGHT HERE!!! THERE'S A DANGER ZONE HERE
							numToSum = numbers.Dequeue();
						}	
					}
					if (numToSum != -1)
					{
						mySum += numToSum;
						Console.WriteLine("Consuming thread #" + threadNumber + " adding " + numToSum + " to its total sum making " + mySum + " for the thread total.");
					}
				}
				sums[(int)threadNumber] = mySum;
			}

			static void SynchronizedContainersMain()
			{
				var producingThread = new Thread(ProduceNumbers);
				producingThread.Start();
				Thread[] threads = new Thread[NumThreads];
				for (int i = 0; i < NumThreads; i++)
				{
					threads[i] = new Thread(SumNumbers);
					threads[i].Start(i);
				}
				for (int i = 0; i < NumThreads; i++)
					threads[i].Join();
				int totalSum = 0;
				for (int i = 0; i < NumThreads; i++)
					totalSum += sums[i];
				Console.WriteLine("Done adding. Total is " + totalSum);
			}
		}

		class MySynchronizedQueue<T>
		{
			object baton = new object();
			Queue<T> theQ = new Queue<T>();
			public void Enqueue(T item)	{
				lock(baton)
					theQ.Enqueue(item);
			}
			public T Dequeue() {
				lock (baton)
					return theQ.Dequeue();
			}
			public int Count {
				get { lock (baton) return theQ.Count; }
			}
			public object SyncRoot {
				get { return baton; }
			}
		}
	}

	// *** 20 *** Locking Code Intelligently - synchronize methods instead of object
	public class LockingCodeIntelligently
	{
		class PublicRestroom
		{
			object baton = new object(); // naive approach, should be able to use stall 1 and stall 2 at the same time
			object stall1baton = new object();
			object stall2baton = new object();
			public void UseStall1() {
				lock (stall1baton) {
					Console.WriteLine("In stall 1");
					Thread.Sleep(2000);
				}
			}
			public void UseStall2() {
				lock (stall2baton) {
					Console.WriteLine("In stall 2");
					Thread.Sleep(2000);
				}
			}
			public void UseSink1() {
				lock (stall1baton) {
					Console.WriteLine("In sink 1");
					Thread.Sleep(2000);
				}
			}
			public void UseSink2() {
				lock (stall2baton) {
					Console.WriteLine("In sink 2");
					Thread.Sleep(2000);
				}
			}
		}

        public class LockingCodeIntelligentlyMainClass
		{
			static void LockingCodeIntelligentlyMain()
			{
				var restroom = new PublicRestroom();
				new Thread(restroom.UseStall1).Start();
				new Thread(restroom.UseStall2).Start();
				new Thread(restroom.UseSink1).Start();
				new Thread(restroom.UseSink2).Start();
			}
		}
	}

	// *** 21 *** Why Locking On this is Bad
	public class WhyLockingOnThisIsBad
	{
		class BathroomStall
		{
			object baton = new object();
			public void BeUsed() {
				lock (baton)
					Console.WriteLine("Doing our thing...");
			}
			public void FlushToilet() {
				lock (baton)
					Console.WriteLine("Flushing the toilet...");
			}
		}

		class PublicRestroom
		{
			BathroomStall stall1 = new BathroomStall();
			BathroomStall stall2 = new BathroomStall();
			public void UseStall1() {
				lock (stall1)
				{
					stall1.BeUsed();
					// when our code is between DANGER ZONE
					stall1.FlushToilet();
				}
			}
			public void UseStall2()	{
				stall2.BeUsed();
				stall2.FlushToilet();
			}
			public void CleanTheSink() {
				lock(stall1) // if above BathroomStall BeUsed and FlushToilet used lock (this), it would prevent us from executing this code
					Console.WriteLine("Clean the sink...");
			}
		}
	}

	// *** 22 *** Locking on this vs MethodImplOptions.Synchronized - MethodImplOptions is the same as locking on this (instance methods) or the object type (static methods)
	// if methods are static, there is no object to lock on, would be identical to lock(typeof(BathroomStall))
	public class LockingOnThisVsMethodImplOptionsSynchronized
	{
		class BathroomStall
		{
			[MethodImpl(MethodImplOptions.Synchronized)] // synchronize this method don't allow more than one thread into this method at the same time
			public void BeUsed()
			{
				Console.WriteLine("Doing our thing...");
				Thread.Sleep(5000);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			public void FlushToilet()
			{
				Console.WriteLine("Flushing the toilet...");
				Thread.Sleep(3000);
			}
		}

        public class LockingOnThisVsMethodImplOptionsSynchronizedMainClass
		{
			static void LockingOnThisVsMethodImplOptionsSynchronizedMain()
			{
				var stall = new BathroomStall();
				new Thread(stall.BeUsed).Start();
				new Thread(stall.FlushToilet).Start();
			}
		}
	}

    #endregion THREADING

    #region ATTRIBUTES_REFLECTION

    /* Attributes provide a powerful method of associating metadata, or declarative information, with code (assemblies, types, methods, properties, and so forth). 
     * After an attribute is associated with a program entity, the attribute can be queried at run time by using a technique called reflection.
     */

    // *** 1 *** Hello World Attributes - build project generates executable, that executable is an assembly (executable or DLL)
    // 3 types inside this assembly (within class HelloWorldAttributes) - TestAttribute, MyTestSuite, HelloWorldAttributesMainClass
    public class HelloWorldAttributes
    {
        class TestAttribute : Attribute { }

        [TestAttribute]
        class MyTestSuite { }

        [TestAttribute]
        class YourTestSuite { }
        
        public class HelloWorldAttributesMainClass
        {
            public static void HelloWorldAttributesMain()
            {   
                // print all types that have TestAttribute
                foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
                    foreach(Attribute a in t.GetCustomAttributes(false))
                        if (a is TestAttribute)
                            Console.WriteLine(t.Name + " is a test suite!");

                // LINQ translation
                var testSuites =
                    from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.GetCustomAttributes(false).Any(a => a is TestAttribute)
                    select t;
                foreach(Type t in testSuites)
                    Console.WriteLine(t.Name);
            }
        }
    }

    // *** 2 *** Reflection - reflection means look at info through Type objects that describes assemblies, modules, and types
    public class Reflection
    {
        class TestAttribute : Attribute { }

        class TestMethodAttribute: Attribute { }

        [Test] // "Attribute" not necessary - automatically types out TestAttribute
        class MyTestSuite 
        {
            public void HelperMethod() { Console.WriteLine("Helps our tests get their job done..."); }
            [TestMethod]
            public void MyTestMethod1() 
            {
                HelperMethod();
                Console.WriteLine("Doing some testing..."); 
            }
            [TestMethod]
            public void MyTestMethod2() 
            {
                HelperMethod();
                Console.WriteLine("Doing some other testing..."); 
            }
        }

        public class ReflectionMainClass 
        {
            public static void ReflectionMain()
            {
                // automatically execute methods with TestMethodAttribute
                // give me the types and inside the types give me the custom attributes
                var testSuites =
                        from t in Assembly.GetExecutingAssembly().GetTypes()
                        where t.GetCustomAttributes(false).Any(a => a is TestAttribute)
                        select t;

                foreach (Type t in testSuites)
                { 
                    Console.WriteLine("Running tests in suite: " + t.Name);
                    var testMethods =
                        from m in t.GetMethods()
                        where m.GetCustomAttributes(false).Any(a => a is TestMethodAttribute)
                        select m;
                    object testSuiteInstance = Activator.CreateInstance(t);
                    foreach (MethodInfo mInfo in testMethods)
                        mInfo.Invoke(testSuiteInstance, new object[0]);
                }
            }
        }

        // *** 3 *** Attributes - we decorate types with attributes
        public class Attributes
        {
            class MeAttribute : Attribute 
            {
                public MeAttribute(int value, string secondValue)
                {
                    Console.WriteLine("MeAttribute");
                    Console.WriteLine(value);
                    Console.WriteLine(secondValue);
                }
                public int SomeIntProperty { get; set; }
                public char SomeCharProperty { get; set; }
            }

            [Me(25, "Yianni", SomeCharProperty = 'J', SomeIntProperty = 72)]
            public class AttributesMainClass
            {
                public static void AttributesMain()
                {
                    typeof(AttributesMainClass).GetCustomAttributes(false);
                }
            }
        }
    }

    // *** 4 *** Attributes Example
    public class AttributesExample
    {
        // Example 1
        [Serializable]
        class Cow
        {
            public string Name;
            public int Weight;
        }

        public class AttributesExampleOneMainClass
        {
            public static void AttributesExampleOneMain()
            {
                // doesn't work unless Cow has Serializable attribute
                var betsy = new Cow { Name = "Betsy", Weight = 500 };
                var formatter = new BinaryFormatter();
                var memoryStream = new MemoryStream();
                formatter.Serialize(memoryStream, betsy);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var betsysClone = formatter.Deserialize(memoryStream) as Cow;
                Console.WriteLine(betsysClone.Name);
                Console.WriteLine(betsysClone.Weight);
            }
        }

        // Example 2
        class MeContext : DbContext
        {
            public DbSet<Person> People { get; set; }
        }

        [Table("ActualTableNameInDatabase")]
        class Person
        {
            public int ID { get; set; }
            [Required]
            [MaxLength(50)]
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
        }

        public class AttributesExampleTwoMainClass
        {
            public static void AttributesExampleTwoMain()
            {
                var db = new MeContext();
                foreach (var person in db.People)
                    Console.WriteLine(person.FirstName);
            }
        }

        // Example 3
        [ServiceContract]
        interface ICow
        {
            [OperationContract]
            void Moo();
        }

        class MeCow : ICow
        {
            public void Moo() { Console.WriteLine("Moooooo"); }
        }

        public class AttributesExampleThreeMainClass
        {
            public static void AttributesExampleThreeMain()
            {
                var host = new ServiceHost(typeof(MeCow));
                host.AddServiceEndpoint(typeof(ICow), new WSHttpBinding(), "http://localhost:1234");
                host.Open();

                // At some other end of the globe:
                var cow = ChannelFactory<ICow>.CreateChannel(new WSHttpBinding(), new EndpointAddress("http://localhost:1234"));
                cow.Moo();
            }
        }
    }

    // *** 5 *** How .NET Embeds Attribute Data Into Assemblies
    public class EmbedsAttributeDataIntoAssemblies
    {
        class MeAttribute : Attribute 
        {
            public MeAttribute(int arg) { }
            public string MeString { get; set; }
        }
        
        // attribute arguments must be constant expressions
        [Me(5 + 3, MeString = "I love programming with attributes")]
        public class EmbedsAttributeDataIntoAssembliesMainClass
        {
            public static void EmbedsAttributeDataIntoAssembliesMain()
            {
            }
        }
    }

    // *** 6 *** Attribute Usage
    public class AttributeUsage
    {
        //[assembly: Me]

        // will only allow attribute usage on classes or fields
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
        class MeAttribute : Attribute 
        {
            public MeAttribute() { Console.WriteLine("MeAttribute()"); }
        }

        [Me, Me, Me, Me]
        class MeVictim
        {
            public MeVictim() { MeField = ""; MeEvent(); }
            //[Me]
            public int MeProperty { /*[Me]*/ get { return 5; } }
            //[Me]
            public event Action MeEvent;
            [Me]
            public string MeField;
            //[Me]
            //[return: Me]
            int MeMethod( /*[Me]*/ int meParameter) { return meParameter; }
        }

        public class AttributeUsageMainClass
        {
            public static void AttributeUsageMain()
            {
                typeof(MeVictim).GetCustomAttributes(false);
            }
        }
    }

    // *** 7 *** Inherited Attributes and IsDefined
    public class InheritedAttributesAndIsDefined
    {
        //[AttributeUsage(AttributeTargets.All, Inherited = false)] // Inherited = false will not allow attribute to be inherited
        class MeAttribute : Attribute 
        {
            public MeAttribute() { Console.WriteLine("MeAttribute()"); }
        }

        [Me]
        class Base { }
        class Derived : Base { }

        public class InheritedAttributesAndIsDefinedMainClass
        {
            public static void InheritedAttributesAndIsDefinedMain()
            {
                Console.WriteLine(typeof(Derived).GetCustomAttributes(true)); // true will give all attributes of inherited classes
                Console.WriteLine(typeof(Derived).IsDefined(typeof(MeAttribute), true)); // tells you if attribute is defined on Derived
            }
        }
    }

    // *** 8 *** Assembly Attributes
    //[assembly: ] // should go on first line of code in file after using statements, attaches attributes to the assembly

    // *** 9 *** ConditionalAttribute - based on certain conditions compile some code in or compile some code out
    //#define WE_BE_DEBUGIN - place on first line of code above using statements
    public class ConditionalAttributeClass
    {
        public class ConditionalAttributeMainClass
        {
            public static void ConditionalAttributeMain()
            {
                TraceDebuggingStuff("We are debugging here");
            }
            [Conditional("WE_BE_DEBUGIN")] // will only compile tagged method TraceDebuggingStuff if we have #define WE_BE_DEBUGIN above
            public static void TraceDebuggingStuff(string messageToTrace)
            {
                Console.WriteLine("Debugging: " + messageToTrace);
            }
        }
    }

    // *** 10 *** ObsoleteAttribute - when we want to deprecate code
    public class ObsoleteAttributeClass
    {
        class MeAwesomeClass
        {
            [Obsolete("Hey, I found a better way to design this API. Please see my new API for a better approach", true)]
            public static void MeFirstAttemptAtAnAwesomeAlgorithm() { Console.WriteLine("Some awesome code"); }
        }
        
        public class ObsoleteAttributeMainClass
        {
            public static void ObsoleteAttributeMain()
            {
                //MeAwesomeClass.MeFirstAttemptAtAnAwesomeAlgorithm(); // provides an error if I try to compile
            }
        }
    }

    // *** 11 *** DebuggerStepThrough and DebuggerHidden Attributes
    public class DebuggerStepThroughDebuggerHiddenAttributes
    {
        class Cow
        {   
            public string Name { [DebuggerStepThrough] get { return "Bessy"; } }
            public int Age { get { return 5; } }
        }

        public class DebuggerStepThroughDebuggerHiddenAttributesMainClass
        {
            public static void DebuggerStepThroughDebuggerHiddenAttributesMain()
            {
                var cow = new Cow();
                EyeCowForButchering(cow.Name, cow.Age);

                FirstMethod();
            }
            static void EyeCowForButchering(string name, int age)
            {
                Console.WriteLine(name + " " + age);
            }

            static void FirstMethod() { SecondMethod(); }
            //[DebuggerStepThrough]
            [DebuggerHidden]
            static void SecondMethod() { ThirdMethod(); }
            static void ThirdMethod() { }
        }
    }

    // *** 12 *** DebuggerDisplayAttribute
    public class DebuggerDisplayAttributeClass
    {
        [DebuggerDisplay("{Name}, Amount of meat: {Weight}")]
        class Cow
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public int Weight { get; set; }
        }

        public class DebuggerDisplayAttributeMainClass
        {
            public static void DebuggerDisplayAttributeMain()
            {
                var cow = new Cow { Name = "Betsy", Age = 5, Weight = 1500 };
                Console.WriteLine(cow);
            }
        }
    }

    // *** 13, 14, 15, 16 *** Add-Ins, .NET Add-Ins, .NET Add-In Host
    // only reference interface - plug-in architecture, at runtime use reflection, late binding - bind at runtime dynamically
    public class AddIns
    {
        public struct ChessMove
        {
            public int StartRow { get; set; }
            public int EndRow { get; set; }
            public int StartColumn { get; set; }
            public int EndColumn { get; set; }
        }
        public enum ChessPiece { King, Queen, Rook, Knight, Bishop, Pawn }
        public interface IChessGame
        {
            ChessMove MakeMove(ChessPiece[,] board);
        }

        class MyChessAlgorithm : IChessGame
        {
            public ChessMove MakeMove(ChessPiece[,] board) { return new ChessMove() { StartRow = 1, EndRow = 2, StartColumn = 3, EndColumn = 4 }; }
        }
        class YourChessAlgorithm : IChessGame
        {
            public ChessMove MakeMove(ChessPiece[,] board) { return new ChessMove() { StartRow = 100, EndRow = 200, StartColumn = 300, EndColumn = 400 }; }
        }

        
        public class AddInsMainClass
        {
            public static void AddInsMain()
            {
                // late binding - load assemblies at runtime
                Assembly player1Assembly = Assembly.Load("MyChessAlgorithm");
                Assembly player2Assembly = Assembly.Load("YourChessAlgorithm");
                IChessGame player1 = CreatePlayerAlgorithmInstance(player1Assembly);
                IChessGame player2 = CreatePlayerAlgorithmInstance(player2Assembly);

                ChessMove myMove = player1.MakeMove(null);
                ChessMove yourMove = player2.MakeMove(null);
                Console.WriteLine(myMove.StartColumn);
                Console.WriteLine(yourMove.StartColumn);
            }

            private static IChessGame CreatePlayerAlgorithmInstance(Assembly player)
            {
                Type playerAlgorithmType = player.GetTypes().Single(t => t.GetInterfaces().Any(interfaceType => interfaceType.Equals(typeof(IChessGame))));
                return Activator.CreateInstance(playerAlgorithmType) as IChessGame;
            }
        }
    }

    // *** 17, 18 *** Attributes and Serialization, Writing Our Own Serializer
    // serialize is to converts to bits and bytes
    public class AttributesSerialization
    {
        // DataContract and DataMember are WCF Attributes
        [DataContract]
        class Person
        {
            [DataMember]
            public string FirstName { get; set; }
            [DataMember]
            public string LastName { get; set; }
            [DataMember]
            public int Age { get; set; }
        }

        class MeSerializer
        {
            Type targetType;
            public MeSerializer(Type targetType)
            {
                this.targetType = targetType;
                if (!targetType.IsDefined(typeof(DataContractAttribute), false))
                    throw new Exception("No soup for you.");
            }
            public void WriteObject(Stream stream, object graph)
            {
                IEnumerable<PropertyInfo> serializableProperties = targetType.GetProperties().Where(p => p.IsDefined(typeof(DataMemberAttribute), false));
                var writer = new StreamWriter(stream);
                writer.WriteLine("<" + targetType.Name + ">");
                foreach (PropertyInfo propInfo in serializableProperties)
                    writer.Write("\t<" + propInfo.Name + ">" + propInfo.GetValue(graph, null) + "</" + propInfo + ">");
                writer.WriteLine("</" + targetType.Name + ">");
                writer.Flush();
            }
        }
        public class AttributesSerializationMainClass
        {
            public static void AttributesSerializationMain()
            {
                var me = new Person { FirstName = "Yianni", LastName = "Alexander", Age = 37 };
                //var serializer = new DataContractSerializer(me.GetType());
                var serializer = new MeSerializer(me.GetType());
                var someRam = new MemoryStream();
                serializer.WriteObject(someRam, me);
                someRam.Seek(0, SeekOrigin.Begin);
                Console.WriteLine(XElement.Parse(Encoding.ASCII.GetString(someRam.GetBuffer()).Replace("\0", "")));
            }
        }
    }

    // *** 19 *** Using Reflection to Analyze Collection Classes
    public class UsingReflectionAnalyzeCollections
    {
        public class UsingReflectionAnalyzeCollectionsMainClass
        {
            public static void UsingReflectionAnalyzeCollectionsMain()
            {
                Assembly mscorlib = Assembly.Load("mscorlib");
                //Console.WriteLine(mscorlib.GetTypes().Where(t => t.Namespace == null ? false : t.Namespace.Contains("System.Collection")).Select(t => t.Name));
                // get count of all types that implement generic IEnumerable
                Console.WriteLine(mscorlib.GetTypes()
                    .Where(t => t.GetInterfaces()
                        .Any(it => it.IsGenericType ? it.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)) : false))
                    .Count());
                // get count of all types that implement generic ICollection
                Console.WriteLine(mscorlib.GetTypes()
                    .Where(t => t.GetInterfaces()
                        .Any(it => it.IsGenericType ? it.GetGenericTypeDefinition().Equals(typeof(ICollection<>)) : false))
                    .Count());
            }
        }
    }

    // *** 20 *** Writing Your Own Reflector
    public class WritingYourOwnReflector
    {
        class Person
        {
            public Person() { Console.WriteLine("Person()"); MeField = 0; GotSomeAction(); }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public int MeField;
            public void AnnounceThyself() { Console.WriteLine("Boooooooyah!"); }
            public event Action GotSomeAction;
        }

        public class WritingYourOwnReflectorMainClass
        {
            public static void WritingYourOwnReflectorMain()
            {
                var assembly = Assembly.GetExecutingAssembly();
                Console.WriteLine(assembly.FullName);
                foreach(Type type in assembly.GetTypes())
                {
                    Console.WriteLine("\t" + type.Name);
                    Console.WriteLine("\t\tFields:");
                    foreach(FieldInfo field in type.GetFields())
                        Console.WriteLine("\t\t\t" + field.FieldType + " " + field.Name);
                    Console.WriteLine("\t\tProperties:");
                    foreach (PropertyInfo prop in type.GetProperties())
                        Console.WriteLine("\t\t\t" + prop.PropertyType + " " + prop.Name);
                    Console.WriteLine("\t\tMethods:");
                    foreach (MethodInfo method in type.GetMethods())
                        Console.WriteLine("\t\t\t" + method.ReturnType + " " + method.Name + "()");
                    Console.WriteLine("\t\tEvents:");
                    foreach(EventInfo eventInfo in type.GetEvents())
                    {
                        Console.WriteLine("\t\t\t" + eventInfo.EventHandlerType + " " + eventInfo.Name);
                        Console.WriteLine("\t\t\t\tAdd method:" + eventInfo.GetAddMethod().Name);
                        Console.WriteLine("\t\t\t\tRemove method:" + eventInfo.GetRemoveMethod().Name);
                    }
                }
            }
        }
    }

    // *** 21, 22 *** Reflection to the Max, MethodInfo, MemberInfo 
    // ORM (Object Relational Mapping) - tables that store object information that can be accessed via "Info" objects
    // "Info" type for every type of member (methods, properties, fields, events, etc..) you can give a class
    public class ReflectionMax
    {
        class Vector
        {
            public float X { get; set; }
            public float Y { get; set; }
            public override string ToString()
            {
                return "{ X: " + X + ", Y: " + Y + " }";
            }
        }
        public class ReflectionMaxMainClass
        {
            public static void ReflectionMaxMain()
            {
                Vector vec = new Vector { X = 4, Y = 9 };
                Console.WriteLine(vec.ToString());

                Type vecType = typeof(Vector);
                Vector vec2 = Activator.CreateInstance(vecType) as Vector;
                PropertyInfo xPropInfo = vecType.GetProperty("X");
                PropertyInfo yPropInfo = vecType.GetProperty("Y");
                xPropInfo.SetValue(vec2, 4, null);
                yPropInfo.SetValue(vec2, 9, null);
                MethodInfo toStringInfo = vecType.GetMethod("ToString");
                string result = toStringInfo.Invoke(vec2, null) as string;
                MethodInfo writeLineInfo = typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });
                writeLineInfo.Invoke(null, new[] { result });
            }
        }
    }

    // *** 23 *** Touching Private Parts, BindingFlags
    public class TouchingPrivateParts
    {
        class Cow
        {
            public string Name { get; set; }
            public int Age { get; set; }
            private int NumHeartBeats { get; set; }
            public void Beat() { NumHeartBeats++; }
            private void Digest() { Console.WriteLine("grind grind..."); }
            static void WhateverStaticMethod() { }
        }

        public class TouchingPrivatePartsMainClass
        {
            public static void TouchingPrivatePartsMain()
            {
                Cow betsy = new Cow { Name = "Betsy", Age = 5 };
                betsy.Beat(); betsy.Beat(); betsy.Beat(); betsy.Beat(); betsy.Beat();
                PropertyInfo propInfo = typeof(Cow).GetProperty("NumHeartBeats", BindingFlags.NonPublic | BindingFlags.Instance);
                int numBeats = (int)propInfo.GetValue(betsy, null);

                //IEnumerable<MemberInfo> members = typeof(Cow).GetMembers().OrderByDescending(mi => mi.DeclaringType.Name);
                // using BFF = System.Reflection.BindingFlags defined at top of file
                IEnumerable<MemberInfo> members = typeof(Cow).GetMembers(BFF.DeclaredOnly | BFF.Instance | BFF.Public | BFF.Static | BFF.NonPublic);
                foreach (MemberInfo minfo in members)
                    Console.WriteLine(minfo.ToString());
            }
        }
    }

    // *** 25 *** Walking the Inheritance Hierarchy
    public class WalkingInheritanceHierarchy
    {
        class MeBase { }
        class MeMid : MeBase { }
        class MeDerived : MeMid { }
        class MeMoreDerived : MeDerived { }
        class MeMegaDerived : MeMoreDerived { }

        public class WalkingInheritanceHierarchyMainClass
        {
            public static void WalkingInheritanceHierarchyMain()
            {
                Type type = typeof(MeMegaDerived);
                while(type != null)
                {
                    Console.WriteLine(type.Name);
                    type = type.BaseType;
                }
            }
        }
    }

    #endregion ATTRIBUTES_REFLECTION

    #region MISCELLANEOUS

    // *** 1 *** var - added in C# 3.5, can only use var within the scope of a function or property body (where you can have expression on right side to infer type)
    // still compile time, still safe, statically checked, useful for anonymous types when we don't know the class name
    public class CSharpVar
	{
		class Cow { public int MooCount { get; set; } }

		class CSharpVarMainClass
		{
			static void CSharpVarMain()
			{
				double dub = 5.3;
				Cow cow = new Cow();
				//var value = 5;
				//List<Cow> cows = new List<Cow>();
				var cows = new List<Cow>();
				var cow2 = new Cow(); // compiler evaluates expression on right side and replaces var with instantiated type
				var cow3 = dub * 2; // cow3 is of type double
			}
		}
	}

	// *** 2 *** Anonymous Types - types that do not have a name, type name generated by compiler, don't do anything except store data, can't put functions in them
	// anonymous types are immutable (once value is set, can't be changed - useful for threading)
	public class AnonymousTypes
	{
		static class AnonymousTypesMainClass
		{
			// compiler creates this class based on the anonymous type 'instance' below
			class CompilerGeneratedForAnonymousType
			{
				public CompilerGeneratedForAnonymousType(string firstName, string lastName, int age, double gpa)
				{
					FirstName = firstName;
					LastName = lastName;
					Age = age;
					GPA = gpa;
				}
				public string FirstName { get; private set; } // set is private because anonymous types are immutable
				public string LastName { get; private set; }
				public int Age { get; private set; }
				public double GPA { get; private set; }
			}
			static void AnonymousTypesMain()
			{
				var instance = new { FirstName = "Yianni", LastName = "Alexander", Age = 36, GPA = 3.5 }; // compiler creates a class (CompilerGeneratedForAnonymousType above)
				var instance2 = new CompilerGeneratedForAnonymousType("Yianni", "Alexander", 36, 3.5);
				Console.WriteLine(instance.LastName);
				Console.WriteLine(instance.FirstName);
				Console.WriteLine(instance.GPA);
				Console.WriteLine(instance2.LastName);
				Console.WriteLine(instance2.FirstName);
				Console.WriteLine(instance2.GPA);
			}
		}
	}

	// *** 3 *** Anonymous Types Other Helper Methods
	public class AnonymousTypesOtherHelperMethods
	{
		static class AnonymousTypesOtherHelperMethodsMainClass
		{
			static void AnonymousTypesOtherHelperMethodsMain()
			{
				var instance = new { FirstName = "Yianni", LastName = "Alexander" };
				var instance2 = new { FirstName = "Yianni", LastName = "Alexander" };
				var instance3 = new { FirstName = "Const", LastName = "Alexander" };
				Console.WriteLine(instance); // compiler generates intuitive ToString()
				Console.WriteLine(instance.GetType()); // type name compiler generates for us
				Console.WriteLine(instance.GetHashCode()); // relies on GetHashCode for data members
				Console.WriteLine(instance2.GetHashCode()); // will have same HashCode as instance
				Console.WriteLine(instance3.GetHashCode()); // will have different HashCode from instance and instance2
				Console.WriteLine(instance.Equals(instance2)); // compiler overrides equals (.Equals is polymorphic, == is static) - returns True
				Console.WriteLine(instance.Equals(instance2)); // returns False
			}
		}
	}

	// *** 4 *** Inferring Anonymous Type Property Names
	public class InferringAnonymousTypePropertyNames
	{
		class InferringAnonymousTypePropertyNamesMainClass
		{
			static void InferringAnonymousTypePropertyNamesMain()
			{
				string firstName = "Yianni";
				string lastName = "Alexander";
				var person = new { FirstName = "Yianni", LastName = "Alexander" }; // property names are FirstName, LastName
				var person2 = new { firstName, lastName }; // property names are firstName, lastName
				var person3 = new { FirstName = firstName, LastName = lastName }; // property names are FirstName, LastName
																				  //var person2 = new { firstName, lastName, firstName + " " + lastName }; // doesn't work since 3rd argument has 5 expressions
				var person4 = new { firstName, lastName, FullName = firstName + " " + lastName }; // forced to be explicit, property names are firstName, lastName, FullName
				Console.WriteLine(person2.firstName);
				Console.WriteLine(person2.lastName);
				Console.WriteLine(person4);
			}
		}
	}

	// *** 5 *** Compiler Generated Anonymous Types Are Generic - anonymous types are generic
	public class CompilerGeneratedAnonymousTypesAreGeneric
	{
		static class CompilerGeneratedAnonymousTypesAreGenericMainClass
		{
			//create generic anonymous type as compiler would
			class at1<T, R>
			{
				public T FirstName { get; set; }
				public R LastName { get; set; }
			}
			class at2<T, R>
			{
				public T FirstNamer { get; set; }
				public R LastName { get; set; }
			}

			static void CompilerGeneratedAnonymousTypesAreGenericMain()
			{
				//var instance = new { FirstName = "Yianni", LastName = "Alexander" };
				var instance = new { FirstName = "Yianni", LastName = "Alexander" };
				var instance2 = new { FirstName = 5.0, LastName = "Alexander" }; // compiler can reuse the same generic anonymous type for both of these because property names are the same
				var instance3 = new { FirstNamer = 5.0, LastName = "Alexander" }; // cannot reuse same generic type because property name is different
				Console.WriteLine(instance.GetType());
				Console.WriteLine(instance2.GetType());
				var _instance = new at1<string, string> { FirstName = "Yianni", LastName = "Alexander" };
				var _instance2 = new at1<double, string> { FirstName = 5.0, LastName = "Alexander" };
				var _instance3 = new at2<double, string> { FirstNamer = 5.0, LastName = "Alexander" };
			}
		}
	}
	#endregion MISCELLANEOUS

	public class RandomStuff
    {
		// Sort using a custom comparison method
		public static string[] SortIntegerStrings(string[] unsorted)
		{
			Array.Sort(unsorted, CompareIntegerStrings);
			return unsorted;
		}
		// returns negative number if x < y (x comes before y), 0 if equal, and positive number if x > y (x comes after y)
		public static int CompareIntegerStrings(string x, string y)
		{
			if (x.Length != y.Length)
				return x.Length - y.Length;
			for (int i = 0; i < x.Length; i++)
			{
				char left = x[i];
				char right = y[i];
				if (left != right)
					return left - right;
			}
			return 0;
		}

		// Example of sorting 2D List on both columns
		public static void PrintCustomerNumberInOrder()
		{
			int[][] orders = new int[5][];
			orders[0] = new int[] { 8, 3 };
			orders[1] = new int[] { 5, 6 };
			orders[2] = new int[] { 6, 2 };
			orders[3] = new int[] { 2, 3 };
			orders[4] = new int[] { 4, 3 };
			int[] customerOrder = GetCustomerNumberOrder(orders);
			Array.ForEach(customerOrder, x => Console.WriteLine(x));
		}
		public static int[] GetCustomerNumberOrder(int[][] orders)
		{
			int n = orders.Length;
			int[] customerOrder = new int[n];
			List<List<int>> ordersList = new List<List<int>>();
			for (int i = 0; i < n; i++)
			{
				List<int> currentCustomer = new List<int>();
				currentCustomer.Add(orders[i][0] + orders[i][1]); // Total prep time
				currentCustomer.Add(i + 1); // Customer #
				ordersList.Add(currentCustomer);
			}

			// Order by total prep time, then by customer #
			ordersList = ordersList.OrderBy(o => o[0]).ThenBy(o => o[1]).ToList();

			for (int i = 0; i < n; i++)
			{
				Console.WriteLine(ordersList[i][0] + ", " + ordersList[i][1]);
				customerOrder[i] = ordersList[i][1];
			}

			return customerOrder;
		}

		public void MainFunc()
        {
            // string multiplication
            string str = new string('*', 10);
			// Find index of item in array
			int[] arr = new int[] { 1, 2, 3, 4, 5 };
			int index = Array.FindIndex(arr, i => i == 3);
		}
	}

    class Program
    {
        static void Main(string[] args)
        {
            List_IComparable_IComparer babeList = new List_IComparable_IComparer();
            babeList.List_IComparable_Comparer_Example();

            //String hostName = string.Empty;
            //hostName = Dns.GetHostName();
            //IPHostEntry myIP = Dns.GetHostEntry(hostName);
            //IPAddress[] address = myIP.AddressList;

            //for (int i = 0; i < address.Length; i++)
            //{
            //    Console.WriteLine($"{address[i].AddressFamily.ToString()}: {address[i].ToString()}");
            //}

            //Console.WriteLine("------------");

            ////string list = string.Join(Environment.NewLine, address.Select(a => $"{a.AddressFamily.ToString()}: {a.ToString()}").ToArray());
            //string list = string.Join(Environment.NewLine, address.Select(a => a.AddressFamily.ToString() + ": " + a.ToString()).ToArray());

            //Console.WriteLine(list);

            Console.ReadKey();
        }
	}
}

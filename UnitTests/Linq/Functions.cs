﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NUnit.Framework;

using BLToolkit.Data.DataProvider;
using BLToolkit.Data.Linq;

namespace Data.Linq
{
	using Model;

	[TestFixture]
	public class Functions : TestBase
	{
		[Test]
		public void Contains1()
		{
			ForEachProvider(db => AreEqual(
				from p in    Parent where new[] { 1, 2 }.Contains(p.ParentID) select p,
				from p in db.Parent where new[] { 1, 2 }.Contains(p.ParentID) select p));
		}

		[Test]
		public void Contains2()
		{
			var arr = new[] { 1, 2 };

			ForEachProvider(db => AreEqual(
				from p in    Parent where arr.Contains(p.ParentID) select p,
				from p in db.Parent where arr.Contains(p.ParentID) select p));
		}

		[Test]
		public void Contains3()
		{
			var n = 2;

			var expected =
				from p in Parent
				where new[] { 1, n }.Contains(p.ParentID)
				select p;

			ForEachProvider(data => AreEqual(expected, CompiledQuery.Compile<TestDbManager,IQueryable<Parent>>(db =>
				from p in db.Parent
				where new[] { 1, n }.Contains(p.ParentID)
				select p)(data)));
		}

		[Test]
		public void Contains4()
		{
			var arr = new[] { 1, 2 };

			var expected =
				from p in Parent
				where arr.Contains(p.ParentID)
				select p;

			ForEachProvider(data => AreEqual(expected, CompiledQuery.Compile<TestDbManager,IQueryable<Parent>>(db =>
				from p in db.Parent
				where arr.Contains(p.ParentID)
				select p)(data)));
		}

		[Test]
		public void Contains5()
		{
			var arr1 = new[] { 1, 2 };
			var arr2 = new[] { 1, 2, 4 };

			var expected1 = from p in Parent where arr1.Contains(p.ParentID) select p;
			var expected2 = from p in Parent where arr2.Contains(p.ParentID) select p;

			ForEachProvider(data =>
			{
				var cq = CompiledQuery.Compile<TestDbManager,int[],IQueryable<Parent>>((db,a) =>
					from p in db.Parent
					where a.Contains(p.ParentID)
					select p);

				AreEqual(expected1, cq(data, arr1));
				AreEqual(expected2, cq(data, arr2));
			});
		}

		[Test]
		public void Contains6()
		{
			var arr = new List<int> { 1, 2 };

			ForEachProvider(db => AreEqual(
				from p in    Parent where arr.Contains(p.ParentID) select p,
				from p in db.Parent where arr.Contains(p.ParentID) select p));
		}

		[Test]
		public void Contains7()
		{
			IEnumerable<int> arr = new[] { 1, 2 };

			ForEachProvider(db => AreEqual(
				from p in    Parent where arr.Contains(p.ParentID) select p,
				from p in db.Parent where arr.Contains(p.ParentID) select p));
		}

		[Test]
		public void EmptyContains1()
		{
			var expected =
				from p in Parent
				where new int[0].Contains(p.ParentID) || p.ParentID == 2
				select p;

			ForEachProvider(db => AreEqual(expected,
				from p in db.Parent
				where new int[0].Contains(p.ParentID) || p.ParentID == 2
				select p));
		}

		[Test]
		public void Equals1()
		{
			ForEachProvider(db => AreEqual(
				from p in    Parent where p.ParentID.Equals(2) select p,
				from p in db.Parent where p.ParentID.Equals(2) select p));
		}

		[Test]
		public void Equals2()
		{
			var child    = (from ch in Child where ch.ParentID == 2 select ch).First();
			var expected = from ch in Child where !ch.Equals(child) select ch;

			ForEachProvider(db => AreEqual(expected, from ch in db.Child where !ch.Equals(child) select ch));
		}

		[Test]
		public void Equals3()
		{
			ForEachProvider(db => AreEqual(
				from p in    Parent where p.Value1.Equals(null) select p,
				from p in db.Parent where p.Value1.Equals(null) select p));
		}

		[Test]
		public void Equals4()
		{
			using (var db = new NorthwindDB())
				AreEqual(
					   Customer.Where(c => !c.Address.Equals(null)),
					db.Customer.Where(c => !c.Address.Equals(null)));
		}

		[Test]
		public void NewGuid1()
		{
			ForEachProvider(new[] { ProviderName.DB2, ProviderName.Informix, ProviderName.PostgreSQL, ProviderName.SQLite, ProviderName.Access }, db => AreEqual(
				from p in    Types where p.GuidValue != Sql.NewGuid() select p.GuidValue,
				from p in db.Types where p.GuidValue != Sql.NewGuid() select p.GuidValue));
		}

		[Test]
		public void NewGuid2()
		{
			ForEachProvider(new[] { ProviderName.DB2, ProviderName.Informix, ProviderName.PostgreSQL, ProviderName.SQLite, ProviderName.Access }, db =>
				Assert.AreNotEqual(Guid.Empty, (from p in db.Types select Sql.NewGuid()).First()));
		}

		[Test]
		public void CustomFunc()
		{
			Expressions.MapMember<Person>(p => p.FullName(), (Expression<Func<Person,string>>)(p => p.LastName + ", " + p.FirstName));

			ForEachProvider(db => AreEqual(
				from p in    Person where p.FullName() == "Pupkin, John" select p.FullName(),
				from p in db.Person where p.FullName() == "Pupkin, John" select p.FullName()));
		}
	}

	public static class PersonExtension
	{
		static public string FullName(this Person person)
		{
			return person.LastName + ", " + person.FirstName;
		}
	}
}

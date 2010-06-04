﻿using System;
using System.Linq;
using Data.Linq;

using NUnit.Framework;

namespace Data.Exceptions
{
	[TestFixture]
	public class ElementOperationTest : TestBase
	{
		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void First()
		{
			ForEachProvider(db => db.Parent.First(p => p.ParentID == 100));
		}

		[Test, ExpectedException(typeof(InvalidOperationException))]
		public void Single()
		{
			ForEachProvider(db => db.Parent.Single());
		}
	}
}

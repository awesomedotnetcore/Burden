﻿using System;
using System.Globalization;
using EPS.Concurrency.Tests.Unit;
using EPS.Test.Redis;
using ServiceStack.Redis;
using Xunit;

namespace EPS.Concurrency.Redis.Tests.Integration
{
	public class RedisValueTypeJobQueueTest
		: RedisJobQueueTest<int, int>
	{
		public RedisValueTypeJobQueueTest()
			: base(input => input * 2)
		{ }
	}

	public class RedisReferenceTypeJobQueueTest
		: RedisJobQueueTest<string, string>
	{
 		public RedisReferenceTypeJobQueueTest()
			: base(input => "fail : " + input)
		{ }	
	}

	public abstract class RedisJobQueueTest<TQueue, TQueuePoison>
		: DurableJobQueueTest<RedisJobQueue<TQueue, TQueuePoison>, TQueue, TQueuePoison>
	{
		private static IRedisClientsManager GetClientManager()
		{
			var redisConnection = RedisHostManager.Current();
			return new BasicRedisClientManager(new string[] { String.Format(CultureInfo.InvariantCulture, "{0}:{1}", redisConnection.Host, redisConnection.Port) });
		}

		protected RedisJobQueueTest(Func<TQueue, TQueuePoison> poisonConverter) :
			base(() => new RedisJobQueue<TQueue, TQueuePoison>(GetClientManager(), QueueNames.Default), poisonConverter)
		{ }

		[Fact]
		public void Constructor_Throws_OnNullClientManager()
		{
			Assert.Throws<ArgumentNullException>(() => new RedisJobQueue<TQueue, TQueuePoison>(null, QueueNames.Default));
		}
		
		[Fact]
		public void Constructor_Throws_OnNullQueueNames()
		{
			Assert.Throws<ArgumentNullException>(() => new RedisJobQueue<TQueue, TQueuePoison>(GetClientManager(), null));
		}
	}
}
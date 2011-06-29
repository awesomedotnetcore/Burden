using System;
using System.Reactive;
using System.Reactive.Concurrency;

namespace EPS.Concurrency
{

	public class AutoJobQueue : ManualJobQueue
	{
		//http://rxpowertoys.codeplex.com/
		int maxConcurrent;

		public AutoJobQueue(int maxConcurrent)
		{
			if (maxConcurrent < 1)
				throw new ArgumentOutOfRangeException("maxConcurrent");

			this.maxConcurrent = maxConcurrent;
		}

		public override IObservable<Unit> Add(Func<IObservable<Unit>> asyncStart)
		{
			return Add(asyncStart, true);
		}

		public IObservable<Unit> Add(Func<IObservable<Unit>> asyncStart, bool autoStart)
		{
			IObservable<Unit> whenCompletes = base.Add(asyncStart);
			if (autoStart)
				StartUpTo(maxConcurrent);
			return whenCompletes;
		}

		protected override void OnJobCompleted(Job job, Exception error)
		{
			base.OnJobCompleted(job, error);
			if (error != null)
				Scheduler.ThreadPool.Schedule(() => StartUpTo(maxConcurrent));
			else
				StartUpTo(maxConcurrent);
		}
	}
}
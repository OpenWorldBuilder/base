﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

// simple benchmark class that benchmarks tests
public class ReGoapBenchmarkTests
{
    private ReGoapTests tests;

    private static double Profile(string description, Action func, int iterations = 100)
    {
        // from: http://stackoverflow.com/questions/1047218/benchmarking-small-code-samples-in-c-can-this-implementation-be-improved
        //Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        //Thread.CurrentThread.Priority = ThreadPriority.Highest;
        // warm up 
        func();

        var watch = new Stopwatch();
        ReGoapLogger.Instance.Level = ReGoapLogger.DebugLevel.None;

        // clean up
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        watch.Start();
        for (int i = 0; i < iterations; i++)
        {
            func();
        }
        watch.Stop();

        // clean up
        GC.Collect();

        ReGoapLogger.Instance.Level = ReGoapLogger.DebugLevel.Full;

        ReGoapLogger.Log(string.Format("[Profile] {0} took {1}ms (iters: {2} ; avg: {3}ms).", description, watch.Elapsed.TotalMilliseconds, iterations, watch.Elapsed.TotalMilliseconds / iterations));
        return watch.Elapsed.TotalMilliseconds;
    }

    [TestFixtureSetUp]
    public void Init()
    {
        tests = new ReGoapTests();
        tests.Init();
    }

    [Test]
    public void SimpleChainedPlanBenchmark()
    {
        Profile("SimpleChainedPlanBenchmark", tests.TestSimpleChainedPlan);
    }

    [Test]
    public void TwoPhaseChainedPlanBenchmark()
    {
        Profile("TwoPhaseChainedPlanBenchmark", tests.TestTwoPhaseChainedPlan);
    }
}

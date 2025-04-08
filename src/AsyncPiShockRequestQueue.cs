using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace REPOShock.src
{
	internal class AsyncPiShockRequestQueue : IDisposable
	{
		private ConcurrentQueue<Func<Task>> _taskQueue;
		private SemaphoreSlim _signal;
		private CancellationTokenSource _cts;
		private Task _worker;
		private readonly object _lock = new();
		private bool _disposed;

		public AsyncPiShockRequestQueue()
		{
			Start();
		}


		public void Enqueue<T>(Func<T, Task> task, T args)
		{
			if (_disposed) throw new ObjectDisposedException(nameof(AsyncPiShockRequestQueue));
			_taskQueue.Enqueue(() => task(args));
			_signal.Release();
		}
		public void Start()
		{
			_taskQueue = new ConcurrentQueue<Func<Task>>();
			_signal = new SemaphoreSlim(0);
			_cts = new CancellationTokenSource();
			_worker = Task.Run(ProcessQueueAsync);
		}

		private async Task ProcessQueueAsync()
		{
			while (!_cts.Token.IsCancellationRequested)
			{
				await _signal.WaitAsync(_cts.Token);

				if (_taskQueue.TryDequeue(out var task))
				{
					try
					{
						await task();
					}
					catch (Exception ex)
					{
						REPOShock.Logger.LogError(ex.ToString());
					}
				}
			}
		}

		public async Task StopAsync()
		{
			if (_disposed) return;

			_cts.Cancel();
			_signal.Release();
			
			try
			{
				await _worker;
			}
			catch (Exception ex)
			{
				REPOShock.Logger.LogError(ex.ToString());
			}
		}

		public async Task ResetAsync()
		{
			lock (_lock)
			{
				if (!_worker.IsCompleted)
				{
					StopAsync().Wait();
				}
				Start();
			}
		}

		public void Dispose()
		{
			if (_disposed) return;

			StopAsync().Wait();
			_cts.Dispose();
			_signal.Dispose();
			_disposed = true;
		}
	}
}

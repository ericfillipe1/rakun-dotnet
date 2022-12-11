using System.Reflection;
using System.Threading.Tasks;
using Rakun.Extensions;

namespace Rakun.Extensions
{

    public static class FluxExtensions
    {
        public static IFlux<R> Flux<R>(Exception exception)
        {
            return new Flux<R>(() => throw exception);
        }

        public static IFlux<T> Flux<T>(this IEnumerable<T> values)
        {
            return new Flux<T>(() => Task.FromResult(values.Select(Task.FromResult)));
        }

        public static IFlux<T> Flux<T>(this Task<IEnumerable<T>> task)
        {
            return new Flux<T>(async () => (await task).Select(Task.FromResult));
        }

        public static IFlux<T> Flux<T>(this Task<List<T>> task)
        {
            return new Flux<T>(async () => (await task).Select(Task.FromResult));
        }

        public static IFlux<R> PipeAsync<T, R>(this IFlux<T> flux, Func<T, Task<R>> mapTo)
        {
            return new Flux<R>(async () =>
            {
                var tasks = await flux.Source();
                return await Task.WhenAll(tasks
                    .Select(t => t.ContinueWith(async (v) => await mapTo(await v))));
            });
        }

        public static async Task<IEnumerable<T>> BuildTask<T>(this IFlux<T> flux)
        {
            var tasks = await flux.Source();
            return await Task.WhenAll(tasks);
        }


        public static IFlux<R> Pipe<T, R>(this IFlux<T> flux, Func<T, R> mapTo)
        {
            return flux.PipeAsync(mapTo.BuildTask);
        }

        public static IFlux<R> PipeMono<T, R>(this IFlux<T> flux, Func<T, IMono<R>> mapTo)
        {
            return flux.PipeAsync(mapTo.BuildTask);
        }

        public static IFlux<T> DoOnNext<T>(this IFlux<T> flux, Action<T> action)
        {
            return flux.PipeAsync((v) =>
            {
                action(v);
                return Task.FromResult(v);
            });

        }

        public static IFlux<T> DoOnError<T>(this IFlux<T> flux, Action<Exception> action)
        {
            return new Flux<T>(async () =>
            {
                var ii = await flux.Source();
                return ii.Select(async task =>
                {
                    var v = await task;
                    try
                    {
                        return v;
                    }
                    catch (Exception e)
                    {
                        action(e);
                        throw e;
                    }
                });
            });

        }


        public static IFlux<T> OnErrorResume<T>(this IFlux<T> flux, Type type, Func<Exception, IMono<T>> func)
        {

            return new Flux<T>(async () =>
            {
                var ii = await flux.Source();
                return ii.Select(async task =>
                {
                    var v = await task;
                    try
                    {
                        return v;
                    }
                    catch (Exception e)
                    {
                        if (type.IsAssignableFrom(e.GetType()))
                        {

                            return await func(e).BuildTask();
                        };
                        throw e;
                    }
                });
            });
        }

        public static IFlux<(T v, R1, R2)> ZipMono<T, R1, R2>(this IFlux<T> flux, IMono<R1> mono1, IMono<R2> mono2)
        {
            return flux.PipeZipAsync((v) => mono1.BuildTask(), (v) => mono2.BuildTask());
        }
        public static IFlux<(T v, R1 Result)> ZipMono<T, R1>(this IFlux<T> flux, IMono<R1> mono1)
        {
            return flux.PipeZipAsync((v) => mono1.BuildTask());
        }


        public static IFlux<(T v, R1)> PipeZipMono<T, R1>(this IFlux<T> flux, Func<T, IMono<R1>> mono1)
        {
            return flux.PipeZipAsync(mono1.BuildTask);
        }

        public static IFlux<(T v, R1, R2)> PipeZipMono<T, R1, R2>(this IFlux<T> flux, Func<T, IMono<R1>> mono1, Func<T, IMono<R2>> mono2)
        {
            return flux.PipeZipAsync(mono1.BuildTask, mono2.BuildTask);
        }
        public static IFlux<(T v, R1, R2, R3)> PipeZipMono<T, R1, R2, R3>(this IFlux<T> flux, Func<T, IMono<R1>> mono1, Func<T, IMono<R2>> mono2, Func<T, IMono<R3>> mono3)
        {
            return flux.PipeZipAsync(mono1.BuildTask, mono2.BuildTask, mono3.BuildTask);
        }

        public static IFlux<(T v, R1, R2, R3, R4)> PipeZipMono<T, R1, R2, R3, R4>(this IFlux<T> flux, Func<T, IMono<R1>> mono1, Func<T, IMono<R2>> mono2, Func<T, IMono<R3>> mono3, Func<T, IMono<R4>> mono4)
        {
            return flux.PipeZipAsync(mono1.BuildTask, mono2.BuildTask, mono3.BuildTask, mono4.BuildTask);
        }
        public static IFlux<(T v, R1, R2, R3, R4, R5)> PipeZipMono<T, R1, R2, R3, R4, R5>(this IFlux<T> flux, Func<T, IMono<R1>> mono1, Func<T, IMono<R2>> mono2, Func<T, IMono<R3>> mono3, Func<T, IMono<R4>> mono4, Func<T, IMono<R5>> mono5)
        {
            return flux.PipeZipAsync(mono1.BuildTask, mono2.BuildTask, mono3.BuildTask, mono4.BuildTask, mono5.BuildTask);
        }

        public static IFlux<(T v, R1 Result)> PipeZipAsync<T, R1>(this IFlux<T> flux, Func<T, Task<R1>> func1)
        {
            return flux.PipeAsync((v) => (Task.FromResult(v), func1(v)).ResolveTask());
        }

        public static IFlux<(T v, R1, R2)> PipeZipAsync<T, R1, R2>(this IFlux<T> flux, Func<T, Task<R1>> func1, Func<T, Task<R2>> func2)
        {
            return flux.PipeAsync((v) => (Task.FromResult(v), func1(v), func2(v)).ResolveTask());
        }

        public static IFlux<(T v, R1, R2, R3)> PipeZipAsync<T, R1, R2, R3>(this IFlux<T> flux, Func<T, Task<R1>> func1, Func<T, Task<R2>> func2, Func<T, Task<R3>> func3)
        {
            return flux.PipeAsync((v) => (Task.FromResult(v), func1(v), func2(v), func3(v)).ResolveTask());
        }

        public static IFlux<(T v, R1, R2, R3, R4)> PipeZipAsync<T, R1, R2, R3, R4>(this IFlux<T> flux, Func<T, Task<R1>> func1, Func<T, Task<R2>> func2, Func<T, Task<R3>> func3, Func<T, Task<R4>> func4)
        {
            return flux.PipeAsync((v) => (Task.FromResult(v), func1(v), func2(v), func3(v), func4(v)).ResolveTask());
        }

        public static IFlux<(T v, R1, R2, R3, R4, R5)> PipeZipAsync<T, R1, R2, R3, R4, R5>(this IFlux<T> flux, Func<T, Task<R1>> func1, Func<T, Task<R2>> func2, Func<T, Task<R3>> func3, Func<T, Task<R4>> func4, Func<T, Task<R5>> func5)
        {
            return flux.PipeAsync((v) => (Task.FromResult(v), func1(v), func2(v), func3(v), func4(v), func5(v)).ResolveTask());
        }
    }
}
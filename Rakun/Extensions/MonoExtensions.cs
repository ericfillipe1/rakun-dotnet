using Rakun.Extensions;

namespace Rakun.Extensions
{

    public static class MonoExtensions
    {
        public static IMono<R> Mono<R>(this Exception exception)
        {
            return new Mono<R>(() => throw exception);
        }

        public static IMono<T> Mono<T>(this T value)
        {
            return new Mono<T>(() => Task.FromResult(value));
        }

        public static IMono<IEnumerable<T>> Enumerable<T>(this IFlux<T> flux)
        {
            return new Mono<IEnumerable<T>>(flux.BuildTask);
        }
        public static IMono Mono(this Task task)
        {
            return new Mono(async () => {
                await task;
                return new MonoVoid { };
            });
        }

        public static IMono<T> Mono<T>(this Task<T> task)
        {
            return new Mono<T>(() => task);
        }

        public static IMono<R> Pipe<T, R>(this IMono<T> mono, Func<T, R> mapTo) =>
                mono.PipeAsync((v) => Task.FromResult(mapTo(v)));

        public static IMono<R> PipeAsync<T, R>(this IMono<T> mono, Func<T, Task<R>> mapTo)
        {
            return new Mono<R>(async () =>
            {
                return await mapTo(await mono.Source());
            });
        }
        public static IMono<R> PipeMono<T, R>(this IMono<T> mono, Func<T, IMono<R>> mapTo)
        {
            return mono.PipeAsync(mapTo.BuildTask);
        }
        public static async Task<T> BuildTask<T>(this IMono<T> mono)
        {
            return await mono.Source();
        }
        public static async Task<R> BuildTask<T1, R>(this Func<T1, IMono<R>> func, T1 t1)
        {
            return await func(t1).Source();
        }

        public static Task<R> BuildTask<T1, R>(this Func<T1, R> func, T1 t1)
        {
            return Task.FromResult(func(t1));
        }
        public static IMono Then<T>(this IMono<T> mono)
        {
            return new Mono(async () =>
            {
                await mono.BuildTask();
                return new MonoVoid { };
            });
        }
        public static IMono<R> Then<T, R>(this IMono<T> mono1, IMono<R> mono2)
        {
            return mono1.PipeAsync((v) => mono2.BuildTask());
        }
        public static IMono<R> ThenReturn<T, R>(this IMono<T> mono1, R mono2)
        {
            return mono1.PipeAsync((v) => Task.FromResult(mono2));
        }
        public static IMono<T> DoOnNext<T>(this IMono<T> mono, Action<T> action)
        {

            return new Mono<T>(async () =>
            {
                var v = await mono.BuildTask();
                action(v);
                return v;
            });

        }
        public static IMono<T> DoOnError<T>(this IMono<T> mono, Action<Exception> action)
        {
            return new Mono<T>(async () =>
            {

                try
                {
                    return await mono.BuildTask();
                }
                catch (Exception e)
                {
                    action(e);
                    throw e;
                }
            });

        }
        public static IMono<T> OnErrorResume<T>(this IMono<T> mono, Type type, Func<Exception, IMono<T>> func)
        {
            return new Mono<T>(async () =>
            {
                try
                {
                    return await mono.BuildTask();
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

        }
        public static IMono<(T v, R1, R2)> ZipMono<T, R1, R2>(this IMono<T> mono, IMono<R1> mono1, IMono<R2> mono2)
        {

            return mono.PipeZipAsync((v) => mono1.BuildTask(), (v) => mono2.BuildTask());

        }
        public static IMono<(T v, R1 Result)> ZipMono<T, R1>(this IMono<T> mono, IMono<R1> mono1)
        {
            return mono.PipeZipAsync((v) => mono1.BuildTask());
        }

        public static IMono<(T v, R1 Result)> PipeZipAsync<T, R1>(this IMono<T> mono, Func<T, Task<R1>> func1)
        {
            return mono.PipeAsync((v) => (Task.FromResult(v), func1(v)).ResolveTask());
        }

        public static IMono<(T v, R1, R2)> PipeZipAsync<T, R1, R2>(this IMono<T> mono, Func<T, Task<R1>> func1, Func<T, Task<R2>> func2)
        {
            return mono.PipeAsync((v) =>(Task.FromResult(v), func1(v), func2(v)).ResolveTask());
        }


        public static IMono<(T v, R1, R2, R3)> PipeZipAsync<T, R1, R2, R3>(this IMono<T> mono, Func<T, Task<R1>> func1, Func<T, Task<R2>> func2, Func<T, Task<R3>> func3)
        {
            return mono.PipeAsync((v) => (Task.FromResult(v), func1(v), func2(v), func3(v)).ResolveTask());
        }


        public static IMono<(T v, R1, R2, R3, R4)> PipeZipAsync<T, R1, R2, R3, R4>(this IMono<T> mono, Func<T, Task<R1>> func1, Func<T, Task<R2>> func2, Func<T, Task<R3>> func3, Func<T, Task<R4>> func4)
        {
            return mono.PipeAsync((v) => (Task.FromResult(v), func1(v), func2(v), func3(v), func4(v)).ResolveTask());
        }
        public static IMono<(T v, R1, R2, R3, R4, R5)> PipeZipAsync<T, R1, R2, R3, R4, R5>(this IMono<T> mono, Func<T, Task<R1>> func1, Func<T, Task<R2>> func2, Func<T, Task<R3>> func3, Func<T, Task<R4>> func4, Func<T, Task<R5>> func5)
        {
           return mono.PipeAsync((v) => (Task.FromResult(v), func1(v), func2(v), func3(v), func4(v), func5(v)).ResolveTask());
        }

        public static IMono<(T v, R1)> PipeZipMono<T, R1>(this IMono<T> mono, Func<T, IMono<R1>> mono1)
        {
            return mono.PipeZipAsync(mono1.BuildTask);
        }

        public static IMono<(T v, R1, R2)> PipeZipMono<T, R1, R2>(this IMono<T> mono, Func<T, IMono<R1>> mono1, Func<T, IMono<R2>> mono2)
        {
            return mono.PipeZipAsync(mono1.BuildTask, mono2.BuildTask);
        }

        public static IMono<(T v, R1, R2, R3)> PipeZipMono<T, R1, R2, R3>(this IMono<T> mono, Func<T, IMono<R1>> mono1, Func<T, IMono<R2>> mono2, Func<T, IMono<R3>> mono3)
        {
            return mono.PipeZipAsync(mono1.BuildTask, mono2.BuildTask, mono3.BuildTask);
        }

        public static IMono<(T v, R1, R2, R3, R4)> PipeZipMono<T, R1, R2, R3, R4>(this IMono<T> mono, Func<T, IMono<R1>> mono1, Func<T, IMono<R2>> mono2, Func<T, IMono<R3>> mono3, Func<T, IMono<R4>> mono4)
        {
            return mono.PipeZipAsync(mono1.BuildTask, mono2.BuildTask, mono3.BuildTask, mono4.BuildTask);
        }

        public static IMono<(T v, R1, R2, R3, R4, R5)> PipeZipMono<T, R1, R2, R3, R4, R5>(this IMono<T> mono, Func<T, IMono<R1>> mono1, Func<T, IMono<R2>> mono2, Func<T, IMono<R3>> mono3, Func<T, IMono<R4>> mono4, Func<T, IMono<R5>> mono5)
        {
            return mono.PipeZipAsync(mono1.BuildTask, mono2.BuildTask, mono3.BuildTask, mono4.BuildTask, mono5.BuildTask);
        }


    }
}
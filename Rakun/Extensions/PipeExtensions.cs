using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakun.Extensions
{
    public static class PipeExtensions
    {


        public static async Task<( R1, R2)> ResolveTask< R1, R2>(this (Task<R1>,Task<R2>) values)
        {
            await Task.WhenAll(values.Item1, values.Item2);
            return (values.Item1.Result, values.Item2.Result);
        }

        public static async Task<(R1, R2, R3)> ResolveTask<R1, R2, R3>(this  (Task<R1>, Task<R2>, Task<R3>) values)
        {
            await Task.WhenAll(values.Item1, values.Item2, values.Item3);
            return (values.Item1.Result, values.Item2.Result, values.Item3.Result);
        }

        public static async Task<(R1, R2, R3, R4)> ResolveTask<R1, R2, R3, R4>(this (Task<R1>, Task<R2>, Task<R3>, Task<R4>) values)
        {
            await Task.WhenAll(values.Item1, values.Item2, values.Item3, values.Item4);
            return (values.Item1.Result, values.Item2.Result, values.Item3.Result, values.Item4.Result);
        }

        public static async Task<(R1, R2, R3, R4, R5)> ResolveTask<R1, R2, R3, R4, R5>(this (Task<R1>, Task<R2>, Task<R3>, Task<R4>, Task<R5>) values)
        {
            await Task.WhenAll(values.Item1, values.Item2, values.Item3, values.Item4, values.Item5);
            return (values.Item1.Result, values.Item2.Result, values.Item3.Result, values.Item4.Result, values.Item5.Result);
        }

        public static async Task<(R1, R2, R3, R4, R5, R6)> ResolveTask<R1, R2, R3, R4, R5, R6>(this (Task<R1>, Task<R2>, Task<R3>, Task<R4>, Task<R5>, Task<R6>) values)
        {
            await Task.WhenAll(values.Item1, values.Item2, values.Item3, values.Item4, values.Item5, values.Item6);
            return (values.Item1.Result, values.Item2.Result, values.Item3.Result, values.Item4.Result, values.Item5.Result, values.Item6.Result);
        }
    }
}

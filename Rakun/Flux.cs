using System;

namespace Rakun
{

    public interface IFlux<T>
    {
        Task<IEnumerable<Task<T>>> Source();

    }

    internal class Flux<T> : IFlux<T>
    {
        private Func<Task<IEnumerable<Task<T>>>> _source;

        public Flux(Func<Task<IEnumerable<Task<T>>>> _source)
        {
            this._source = _source;
        }

        public Task<IEnumerable<Task<T>>> Source()
        {
            return _source();
        }
    }
}
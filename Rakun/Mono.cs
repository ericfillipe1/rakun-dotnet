
namespace Rakun
{
    public interface IMono : IMono<MonoVoid>
    {
    
    
    }
    public interface IMono<T>
    {
        Task<T> Source();
    }



    internal class Mono : Mono<MonoVoid>, IMono
    {
        internal Mono(Func<Task<MonoVoid>> func) : base(func)
        {
        }
    }
    internal class Mono<T> : IMono<T>
    {
        private Func<Task<T>> _source;

        public Mono(Func<Task<T>> _source)
        {
            this._source = _source;
        }

        public Task<T> Source()
        {
            return _source();
        }
    }
}
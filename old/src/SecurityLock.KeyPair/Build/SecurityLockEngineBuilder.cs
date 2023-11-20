namespace SecurityLock.KeyPair;

public sealed partial class KeyPairLockEngine
{
    public sealed class KeyPairLockEngineBuilder
    {
        private KeysRepository _repo;
        private KeyPairLockEngine _engine;

        public KeyPairLockEngineBuilder(
            KeysRepository? repo = null
        )
        {
            _repo = repo ?? new MemoryKeysRepository();
            _engine = new KeyPairLockEngine(new List<ILock>());
        }

        public KeyPairLockEngineBuilder(
            string context,
            KeysRepository? repo = null
        )
        {
            _repo = repo ?? new MemoryKeysRepository(context);
            _engine = new KeyPairLockEngine(context, new List<ILock>());
        }

        public KeyPairLockEngine Build()
        {
            return _engine;
        }

        public static implicit operator KeyPairLockEngine(KeyPairLockEngineBuilder builder)
            => builder.Build();

        public KeyPairLockEngineBuilder AddLock(ILock _lock)
        {
            _engine.AddLock(_lock);
            return this;
        }

        // fazer uma AllowList -> travar para quem não está na lista

        public KeyPairLockEngineBuilder UseBlockList(ForKey forKey, IBlockList blockList) // por exemplo um txt
        {
            ILock l = new Blocker(blockList);
            AddLock(InvertKeysIfUseKeyB(forKey, l));
            return this;
        }

        public KeyPairLockEngineBuilder UseCombinationLimit(ForKey forKey, int limit, TimeSpan expiresIn)
        {
            ILock l = new CombinationLimiter(_repo, limit, expiresIn);
            AddLock(InvertKeysIfUseKeyB(forKey, l));
            return this;
        }

        public KeyPairLockEngineBuilder UseRateLimit(ForKey forKey, int limit, TimeSpan period)
        {
            ILock l = new RateLimiter(_repo, limit, period);
            AddLock(InvertKeysIfUseKeyB(forKey, l));
            return this;
        }

        public KeyPairLockEngineBuilder UsePairOfKeysRateLimit(int limit, TimeSpan period)
        {
            AddLock(new KeysConcatenatorDecorator(new RateLimiter(_repo, limit, period)));
            return this;
        }

        private ILock InvertKeysIfUseKeyB(ForKey forKey, ILock l)
        {
            if (ForKey.B.Equals(forKey))
            {
                l = new KeysInverterDecorator(l);
            }

            return l;
        }
    }
}

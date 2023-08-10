namespace ReproducibleRandomGenerator
{
    public interface IRandomProvider
    {
        void SetSeed(ulong seed);
        float Random();
        RandomCache Random(int index);
    }
}
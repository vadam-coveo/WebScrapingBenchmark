namespace WebscrapingBenchmark.Core.Framework.ChromeDriver
{
    public interface IChromeDriverWrapperFactory
    {
        public IChromeDriverWrapper Create();
        public void Release(IChromeDriverWrapper instance);
    }
}

namespace CoverflowAltTab.Core.Services;

public static class SelectionNavigator
{
    public static int MoveNext(int currentIndex, int count)
    {
        if (count <= 1)
        {
            return 0;
        }

        return (currentIndex + 1) % count;
    }

    public static int MovePrevious(int currentIndex, int count)
    {
        if (count <= 1)
        {
            return 0;
        }

        return (currentIndex - 1 + count) % count;
    }
}

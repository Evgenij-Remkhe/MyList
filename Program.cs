SortWords("a2 a1.A0 fvf A5");

return;

void SortWords(string text)
{
    ReadOnlySpan<char> span = text.AsSpan();
    Span<Word> words = stackalloc Word[span.Length / 2 + 1];
    int wordCount = 0;

    int lastIndex = 0;
    for (int i = 0; i <= span.Length; i++)
    {
        if(i == span.Length || span[i] == ' ' || span[i] == ',' || span[i] == '.' || span[i] == '!' || span[i] == '?')
        {
            ReadOnlySpan<char> word = span.Slice(lastIndex, i - lastIndex);
            if (word.Length == 0 || !(word[0] == 'a' || word[0] == 'A' ))
            {
                lastIndex = i + 1;
                continue;
            }

            words[wordCount++] = new Word(lastIndex, word.Length);
            lastIndex = i + 1;
        }
    }

    if (wordCount == 0)
    {
        return;
    }
    
    for (int i = 0; i < wordCount - 1; i++)
    {
        for (int j = 0; j < wordCount - i - 1; j++)
        {
            ReadOnlySpan<char> word1 = span.Slice(words[j].Start, words[j].Length);
            ReadOnlySpan<char> word2 = span.Slice(words[j + 1].Start, words[j + 1].Length);

            if (word1.CompareTo(word2, StringComparison.Ordinal) > 0)
            {
                (words[j], words[j + 1]) = (words[j + 1], words[j]);
            }
        }
    }

    for (int i = 0; i < wordCount; i++)
    {
        ReadOnlySpan<char> word = span.Slice(words[i].Start, words[i].Length);
        foreach (var c in word)
        {
            Console.Write(c);
        }
        Console.WriteLine();
    }
}

public readonly struct Word
{
    public readonly int Start;
    public readonly int Length;

    public Word(int start, int length)
    {
        Start = start;
        Length = length;
    }
}
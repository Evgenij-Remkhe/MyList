class Program
{
    static void Main(string[] args)
    {
        //Input
        SortWords("a2 a1.A0 fvf A5", 'A');
        /*
         * Output:
         * A0
         * A5
         * a1
         * a2
         */
    }

    private static void SortWords(string text, char firstChar)
    {
        ReadOnlySpan<char> span = text.AsSpan();
        Span<Word> words = stackalloc Word[1];
        int wordCount = 0;

        int lastIndex = 0;
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == ' ' || span[i] == ',' || span[i] == '.' || span[i] == '!' || span[i] == '?')
            {
                ReadOnlySpan<char> word = span.Slice(lastIndex, i - lastIndex);
                if (word.Length == 0 || !char.ToLower(word[0]).Equals(char.ToLower(firstChar)))
                {
                    lastIndex = i + 1;
                    continue;
                }

                if (wordCount >= words.Length)
                {
                    Span<Word> buff = words;
                    words = stackalloc Word[words.Length * 2];
                    for (int j = 0; j < buff.Length; j++)
                    {
                        words[j] = buff[j];
                    }
                }

                words[wordCount++] = new Word(lastIndex, word.Length);
                lastIndex = i + 1;
            }
        }

        ReadOnlySpan<char> lastWord = span.Slice(lastIndex, span.Length - lastIndex);

        if (lastWord.Length > 0 && char.ToLower(lastWord[0]).Equals(char.ToLower(firstChar)))
        {
            if (wordCount >= words.Length)
            {
                Span<Word> buff = words;
                words = stackalloc Word[words.Length * 2];
                for (int j = 0; j < buff.Length; j++)
                {
                    words[j] = buff[j];
                }
            }

            words[wordCount++] = new Word(lastIndex, lastWord.Length);
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
            Console.WriteLine(word.ToString());
        }
    }
}

public struct Word
{
    public readonly int Start;
    public readonly int Length;

    public Word(int start, int length)
    {
        Start = start;
        Length = length;
    }
}

﻿namespace Unima.Core.Git
{
    public interface IGitDiff
    {
        string GetDiff(string path, string branch);
    }
}

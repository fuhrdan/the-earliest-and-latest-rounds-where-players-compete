//*****************************************************************************
//** 1900. The Earliest and Latest Rounds Where Players Compete     leetcode **
//*****************************************************************************

/**
 * Note: The returned array must be malloced, assume caller calls free().
 */
 /*
 // Slow code, do not use
int minRound, maxRound;

// Recursive DFS to simulate the tournament
void dfs(int* players, int size, int round, int firstPlayer, int secondPlayer)
{
    int i, j;
    for (i = 0; i < size; ++i)
    {
        if (players[i] == firstPlayer) break;
    }
    for (j = 0; j < size; ++j)
    {
        if (players[j] == secondPlayer) break;
    }

    if (i + j == size - 1) // will meet this round
    {
        if (round < minRound) minRound = round;
        if (round > maxRound) maxRound = round;
        return;
    }

    int pairs = size / 2;
    int total = 1 << pairs; // simulate all outcomes

    for (int mask = 0; mask < total; ++mask)
    {
        int next[28];
        int idx = 0;

        for (int p = 0; p < pairs; ++p)
        {
            int left = players[p];
            int right = players[size - 1 - p];

            // if either is first/secondPlayer, they win
            if (left == firstPlayer || left == secondPlayer)
                next[idx++] = left;
            else if (right == firstPlayer || right == secondPlayer)
                next[idx++] = right;
            else
                next[idx++] = ((mask >> p) & 1) ? left : right;
        }

        if (size % 2 == 1) // odd number, middle advances
            next[idx++] = players[size / 2];

        // sort next[] to maintain ascending order
        for (int a = 0; a < idx - 1; ++a)
        {
            for (int b = a + 1; b < idx; ++b)
            {
                if (next[a] > next[b])
                {
                    int tmp = next[a];
                    next[a] = next[b];
                    next[b] = tmp;
                }
            }
        }

        dfs(next, idx, round + 1, firstPlayer, secondPlayer);
    }
}

int* earliestAndLatest(int n, int firstPlayer, int secondPlayer, int* returnSize)
{
    int* retval = malloc(2 * sizeof(int));
    *returnSize = 2;

    int players[28];
    for (int i = 0; i < n; ++i)
        players[i] = i + 1;

    minRound = INT_MAX;
    maxRound = INT_MIN;

    dfs(players, n, 1, firstPlayer, secondPlayer);

    retval[0] = minRound;
    retval[1] = maxRound;
    return retval;
}
*/
int memo[29][29][29][2]; // memo[l][r][n][0:min,1:max]

void dfs(int l, int r, int n, int* minRes, int* maxRes)
{
    if (l + r == n + 1)
    {
        *minRes = *maxRes = 1;
        return;
    }

    if (memo[l][r][n][0] != -1)
    {
        *minRes = memo[l][r][n][0];
        *maxRes = memo[l][r][n][1];
        return;
    }

    int minRound = INT_MAX, maxRound = INT_MIN;

    int half = n / 2;
    int totalComb = 1 << half;

    for (int mask = 0; mask < totalComb; ++mask)
    {
        int next[28], idx = 0;

        for (int i = 0; i < half; ++i)
        {
            int a = i + 1;
            int b = n - i;

            if (a == l || b == l)
                next[idx++] = l;
            else if (a == r || b == r)
                next[idx++] = r;
            else
                next[idx++] = ((mask >> i) & 1) ? a : b;
        }

        if (n % 2 == 1)
            next[idx++] = (n + 1) / 2;

        // sort for order preservation
        for (int i = 0; i < idx - 1; ++i)
            for (int j = i + 1; j < idx; ++j)
                if (next[i] > next[j])
                {
                    int tmp = next[i];
                    next[i] = next[j];
                    next[j] = tmp;
                }

        // find new positions of l and r
        int newL = -1, newR = -1;
        for (int i = 0; i < idx; ++i)
        {
            if (next[i] == l) newL = i + 1;
            if (next[i] == r) newR = i + 1;
        }

        if (newL != -1 && newR != -1)
        {
            int subMin, subMax;
            dfs(newL, newR, idx, &subMin, &subMax);
            if (subMin + 1 < minRound) minRound = subMin + 1;
            if (subMax + 1 > maxRound) maxRound = subMax + 1;
        }
    }

    memo[l][r][n][0] = minRound;
    memo[l][r][n][1] = maxRound;
    *minRes = minRound;
    *maxRes = maxRound;
}

int* earliestAndLatest(int n, int firstPlayer, int secondPlayer, int* returnSize)
{
    *returnSize = 2;
    int* res = malloc(sizeof(int) * 2);

    if (firstPlayer > secondPlayer)
    {
        int tmp = firstPlayer;
        firstPlayer = secondPlayer;
        secondPlayer = tmp;
    }

    memset(memo, -1, sizeof(memo));

    int minRes, maxRes;
    dfs(firstPlayer, secondPlayer, n, &minRes, &maxRes);

    res[0] = minRes;
    res[1] = maxRes;
    return res;
}
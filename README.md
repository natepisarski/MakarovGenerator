# Generator
Welcome to MakarovGenerator, a tool for generating (somewhat) coherent text using [markov chains](https://en.wikipedia.org/wiki/Markov_chain). The quality of the generated text won't be as good as something like GPT, but it will still be better than a monkeys-on-a-typewriter approach.

In short, a Markov chain says that the future state depends only on the current and previous states. So, in this sentence:

    I am an example sentence. I am a good example sentence.

The probability that "I" is followed by "am" is 100%, but the probability that "am" is proceeded by "an" is only 50%. This probability is run through a random number generator to create state chains.

## Why?
This lets us do neat things. We can train it on particular data sets to get different kind of data out. We can use it to generate testing data for text processing. We can create bots (which is how SubredditSimulator on Reddit works), the list goes on!

# How does this program work?
The calling convention is a little complicated, but the easiest way is to do this from a command-line:

    MakarovGenerator.exe evaluate filename 5 in

which will evaluate a Markov chain 5 elements long from filename.

It can also be used as a lightweight server:


    MakarovGenerator distributor rootDirectory

The server will begin listening for commands. The commands are sent from the client (same executable). They contain the state you want to start with, how long of a chain you want to generate, etc.

The server has automatic caching, so hashed text files will appear in the directory - this is normal!

# Current Status
The project works, as in - it has worked at least once. Not much extra development ever happened to it, so ancient bugs are probably still lurking inside it. That said, it really is a fully-featured markov chain implementation.

# Project TODO
These are some things I wanted to do, which I just never got around to:

* TODO: Let Markov chains dynamically add new elements (Markov.cs - HumDrum)
* TODO: Let MarkovServer / MarkovDistributor generate random chains (MarkovDistributor.cs)
* TODO: Add checking to see if dynamic additions need to happen and do it if they do
* TODO: Make it not break over improperly formatted stuff
* TODO: Add formatting to the sentences generated

# License
BSD 3-clause

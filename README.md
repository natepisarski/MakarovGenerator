# MakarovGenerator
Welcome to MakarovGenerator, the statistical analysis tool for Markov Chains. In short, a Markov chain says that the future state depends only on the current and previous states. What does this mean exactly? It means that in this sentence:

    I am an example sentence. I am a good example sentence.

The probability that "I" is followed by "am" is 100%, but the probability that "am" is proceeded by "an" is only 50%. This probability is run through a random number generator to create chains of probabilistic state.

## Why?
This lets us do some obviously cool things. It lets us generate sentences that seem life like. This can be used - testing data, data mining, text prediction (think: autocorrect), or abused - human-like bots, spam, etc.

# How does this program work?
This program is a bit complex if you just want something to evaluate / have fun with Markov Chains. The simplest way to use the program for that purpose is to do this:

    MakarovGenerator.exe evaluate filename 5 in

which will evaluate a Markov chain 5 elements long from filename.

It can also be used as a server with automatic memory-saving checking.


    MakarovGenerator distributor rootDirectory

will begin MakarovGenerator in distributor mode. This means that it will look over rootDirectory and listen for commands. The commands are sent from the client (same executable). They contain the state you want to start with, how long of a chain you want to generate, etc. The text you give it is hashed and placed into 0.txt in the directory it's hashed to.... wow, wait, wut?

This is to save space. If you give it the same text twice in a row, it won't have to download all of it again. This has web service in mind.


# Current Status
It's worked a couple times, but it sure is clunky and extraordinarily easy to break.

# Project TODO

* TODO: Let Markov chains dynamically add new elements (Markov.cs - HumDrum)
* TODO: Let MarkovServer / MarkovDistributor generate random chains (MarkovDistributor.cs)
* TODO: Add checking to see if dynamic additions need to happen and do it if they do
* TODO: Make it not break over improperly formatted stuff
* TODO: Add formatting to the sentences generated



# License
BSD 3-clause

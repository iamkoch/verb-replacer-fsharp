# Attempt at a simple Abstract Syntax Tree in F#

Base on [13 ways of looking at a turtle](https://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle-2/#way13) by Scott Wlaschin.

The idea was to:
    
    1. Take the args
    2. Parse them into sentences (I did this by simply joining the args then splitting on full stops. Not foolproof given punctuation in quotes)
    3. Look up replacements for the verbs
    4. Print out the revised text

The program ended up looking like this:

```f#
let program =
    Parse (Text "I walked home. I entered the house. I didn't want to do it, but I decided I had to. I took a huge shit on the dining room table.", fun response ->
    Process (fun () ->
    Print (fun () ->
    Stop)))
```

I used the quite frankly astounding [DataMust API](https://www.datamuse.com/api/) -  astounding because it's fast, free and has great features. Just not so great for what I needed.

I started out with this sentence:

> "I walked home. I entered the house. I didn't want to do it, but I decided I had to. I took a huge shit on the dining room table."

After a couple of hours hacking, my output looked like this in the end, which made me chuckle:

> "I wandered home. I opened first house. I aak need coughs come try only I opted I got to. I gave a huge damn add first eating sitting table."


The full output and replacements is below. It seems the API _always_ returns something, which is a pain. Also, words like "dining" get replaced by "sitting" because they're still verbs.

```
Start I walked home. I entered the house. I didn't want to do it, but I decided I had to. I took a huge shit on the dining room table.
I walked home. I entered the house. I didn't want to do it
 but I decided I had to. I took a huge shit on the dining room table.
original = I; replacement = I;
original = walked; replacement = wandered;
original = home.; replacement = home.;
original = I; replacement = I;
original = entered; replacement = opened;
original = the; replacement = first;
original = house.; replacement = house.;
original = I; replacement = I;
original = didn't; replacement = aak;
original = want; replacement = need;
original = to; replacement = coughs;
original = do; replacement = come;
original = it; replacement = try;
original = ; replacement = ;
original = but; replacement = only;
original = I; replacement = I;
original = decided; replacement = opted;
original = I; replacement = I;
original = had; replacement = got;
original = to.; replacement = to.;
original = I; replacement = I;
original = took; replacement = gave;
original = a; replacement = a;
original = huge; replacement = huge;
original = shit; replacement = damn;
original = on; replacement = add;
original = the; replacement = first;
original = dining; replacement = eating;
original = room; replacement = sitting;
original = table.; replacement = table.;
I wandered home. I opened first house. I aak need coughs come try only I opted I got to. I gave a huge damn add first eating sitting table.
```

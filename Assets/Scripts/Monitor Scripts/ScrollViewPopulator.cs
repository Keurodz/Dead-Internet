using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ScrollViewPopulator : MonoBehaviour
{
    [Header("Scroll View UI Prefabs")]
    public GameObject commentPrefab;
    public GameObject postPrefab;

    public Transform contentPanel; // The content panel of the ScrollView
    public int numberOfItems = 10; // Number of items to populate in the ScrollView

    List<string> presetPostTitles = new List<string>{
        "how to use a microwave",
        "why is this in my recommended again",
        "can someone explain what’s happening here",
        "family secret recipe",
        "24-hour in a walmart challenge",
        "the moon landing was faked",
        "do not google this",
        "putting my toaster in the bathtub",
        "just vibing",
        "how is this not banned?",
        "vines to cure depression",
        "the olive garden bread sticks are fire",
        "i accidentally joined a cult",
        "chat, is this real?",
        "burger king foot lettuce",
        "POV: you just took the last fry from the bag",
        "da fuq is this?",
        "what came first, the chicken or the egg?",
        "how to cheat on your final exam",
        "this escalated quickly",
        "only in ohio bruh",
        "a perfectly normal video (not clickbait)",
        "florida man caught doing THIS on the golf course",
        "what they don’t teach you in school",
        "proof: aliens exist",
        "how to make a computer in minecraft",
        "top 10 action movies of all time",
        "buzzfeed quizzes",
        "disney stars then vs now",
        "tutorial: how to tie a tie",
        "when life gives you lemons",
        "i took it too far",
        "the forbidden tutorial",
        "somehow this makes sense",
    };

    List<string> presetPostDescriptions = new List<string>{
        "i don’t remember making this but here it is",
        "please don’t ask how this happened",
        "recorded this at 4am after 3 cans of monster",
        "this is not a joke. or maybe it is",
        "inspired by a dream i had and also a fever",
        "no context needed. just vibes",
        "this video cost me my sanity and half a sandwich",
        "i legally can’t explain what’s going on here",
        "don’t try this unless you're ready to transcend",
        "dedicated to the thing in my attic",
        "yes this is real. yes i’m scared",
        "made this instead of going to therapy",
        "if you understand this, you might be in danger",
        "i blacked out and woke up to this on my phone",
        "not sure if this is art or a cry for help",
        "filmed using a toaster and pure chaos",
        "this is what happens when you ignore the warnings",
        "featuring: my cat, a spoon, and regret",
        "they told me not to post this. i didn’t listen",
        "this took 7 hours and several existential crises",
        "just a normal tuesday in the simulation",
        "can someone please explain what i made",
        "shoutout to the voice in my head for this idea",
        "i thought this was a good idea. i was wrong",
        "the police said i should take this down",
        "recovered from an encrypted flash drive",
        "the algorithm demanded a sacrifice, so here it is",
        "not affiliated with any known reality",
        "powered by caffeine, chaos, and YouTube tutorials",
        "this came to me in a vision at the gas station",
        "proof that i should not be left unsupervised",
        "100% organic nonsense, gluten-free chaos",
        "you had to be there. or maybe not",
        "don’t worry, no laws were technically broken",
        "found this while trying to delete system32",
        "this might be a cry for help disguised as content",
        "please don't show this to my boss",
        "they told me i couldn't do it. they were right",
        "warning: contains mild eldritch vibes",
        "i swear this made sense when i uploaded it",
        "the internet wasn’t ready for this. neither was i",
        "watch until the end. or don’t. your call",
        "i tried my best. my best was chaos",
        "inspired by true events and zero sleep",
        "submitted for your confusion",
        "unedited and unholy",
        "yes, there is a backstory. no, i won’t tell it",
        "i had a plan, then the ducks intervened",
        "i can't explain it but i stand by it",
        "this is what happens when the void stares back"
    };


    List<string> presetComments = new List<string>{
        "wow, this is so inspiring!",
        "thank you for sharing",
        "LOL",
        "I can't believe this is real",
        "this is so relatable",
        "i don't get it, can someone explain?",
        "this is so funny",
        "liked!",
        "this is so sad",
        "so true",
        "nahhh, i don't believe this",
        "bro what",
        "this changed my life",
        "my grandma showed me this",
        "same",
        "this deserves more likes",
        "i’m crying rn",
        "i was today years old when i learned this",
        "internet was a mistake",
        "how is this not viral yet?",
        "this unlocked a memory",
        "sent this to my dog",
        "i’ve watched this 10 times",
        "thanks, i hate it",
        "wtf did i just watch",
        "AI made this",
        "this feels illegal to watch",
        "anyone else here in 2025?",
        "deep",
        "the algorithm brought me here",
        "i can’t stop watching this",
        "this radiates chaotic energy",
        "bruh moment",
        "i need this framed",
        "this is cursed",
        "i've seen this in a dream",
        "this is my roman empire",
        "my brain",
        "me after watching this",
        "lowkey true tho",
        "this is peak internet",
        "i showed this to my fish",
        "this is giving me secondhand embarrassment",
        "no thoughts, just vibes",
        "they don’t make content like this anymore",
        "this is not real life",
        "the prophecy was true",
        "this has main character energy",
        "unironically genius",
        "i'm scared",
        "how did we get here",
        "this is why aliens won’t visit us",
        "take my data, just show me more of this",
        "this is what the internet is for",
        "me watching this at 3am",
        "help",
        "what dimension is this from",
        "this healed me",
        "this broke me",
        "idk what i just watched but i liked it",
        "honestly same",
        "same energy as a toaster in a bathtub",
        "you win the internet today",
        "this is the content i crave",
        "a masterpiece",
        "what did i just read",
        "it’s giving... something",
        "this is the final boss of content",
        "chaotic neutral energy",
        "this feels like a fever dream",
        "who let this happen",
        "this is oddly comforting",
        "the vibes are off",
        "chef’s kiss",
        "this goes hard, might delete later",
        "me trying to understand",
        "algorithm doing god’s work",
        "bro is spitting facts",
        "i’m scared but intrigued",
        "this feels like the backrooms",
        "this is the most internet thing ever",
        "send help",
        "i need context",
        "this looks AI-generated",
        "this has strong NPC energy",
        "this shouldn't exist but i'm glad it does",
        "you had one job",
        "instant classic",
        "this post cured my depression",
        "i showed this to my therapist",
        "i think i lost brain cells",
        "certified hood classic",
        "why is this so accurate",
        "this post goes hard, feel free to screenshot",
        "404: meaning not found",
        "was not emotionally prepared for this",
        "someone explain like i'm 5",
        "i want this on a t-shirt",
        "this should be illegal",
        "truly the pinnacle of human achievement",
        "this is definitely someone's sleep paralysis demon",
        "ok but why tho",
        "this is art",
        "i feel personally attacked",
        "i wish i could unread this",
        "do it for the plot"
    };


    void Start()
    {
        PopulateScrollView();
    }

    // Populate the ScrollView with content
    void PopulateScrollView()
    {
        // Clear any existing items
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new items
        for (int i = 0; i < numberOfItems; i++)
        {
            GenerateRandomPost();
        }
    }

    // Generates a random post, assuming the prefab is a 'Post'
    private GameObject GenerateRandomPost()
    {
        GameObject postPrefabItem = Instantiate(postPrefab, contentPanel);

        TextMeshProUGUI[] itemTexts = postPrefabItem.GetComponentsInChildren<TextMeshProUGUI>();

        // removes the image panel from the post prefab
        Transform imagePanel = postPrefabItem.transform.Find("Post Content/Image Panel");
        if (imagePanel != null)
        {
            imagePanel.gameObject.SetActive(false);
        }
        
        // attempts to populate the post prefab with texts
        if (itemTexts.Length >= 3) {
            // title header text
            string randomHeader = presetPostTitles[Random.Range(0, presetPostTitles.Count)];
            itemTexts[0].text = randomHeader;

            // post content text
            string randomContent = presetPostDescriptions[Random.Range(0, presetPostDescriptions.Count)];
            itemTexts[1].text = randomContent;
        }

        // attempts to populate the post with random number of comments
        int numberOfComments = Random.Range(1, 5);
        Transform commentPanel = postPrefabItem.transform.Find("Comments").gameObject.transform;
        for (int i = 0; i < numberOfComments; i++)
        {
            GameObject commentPrefabItem = GenerateRandomComment(commentPanel);
        }

        return postPrefabItem;
    }

    // generates and returns a random comment placed in the given comment panel
    private GameObject GenerateRandomComment(Transform commentPanel) {
        string randomComment = presetComments[Random.Range(0, presetComments.Count)];
        GameObject commentPrefabItem = Instantiate(commentPrefab, commentPanel);
        TextMeshProUGUI commentText = commentPrefabItem.GetComponentInChildren<TextMeshProUGUI>();

        if (commentText != null)
        {
            commentText.text = randomComment;
        }
        return commentPrefabItem;
    }
}

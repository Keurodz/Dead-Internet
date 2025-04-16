== AlienFirstMeetingInDungeon ==
 ~PLAY_MUSIC("Puzzle")
~ monologue = true
# Character: Alien
~ monologue = true
- Woah! Who are you supposed to be?!

# Character: None
- You look around in bewilderment and then focus on the strange alien creature before you.

# Character: Innozen
~ monologue = true
- How did I get here? What is this place? How is this possible?

 ~PLAY_MUSIC("Horror")
 
 # Character: None
- Without hesitation, you push past the alien and look around at the strange world before you.

- The world almost seems like the insides of a computer . . . Wires stretch across the walls, electricity brimming through them.

- The walls are pristine and a refurbished gray, resembling the cold hallways one would walk through in a hospital. 

- You try to run your finger across the wall to see if they are physically real and this isn’t just some dream.

- Immediately, the you feel an electric shock course through your body and you quickly remove your finger from the wall. 

- The Alien frantically rushes over to you.

# Character: Alien
~ monologue = false
- $w(Careful!) Don’t touch the walls. There seems to be some kind of weird barrier that wont let me move from this place. I’ve been trying myself, but I keep getting shocked just like you did.

 ~PLAY_MUSIC("Alien")
 

# Character: None
- While thinking, you stare intently at the Alien. 

# Character: Innozen
~ monologue = true
- Who is this person with blue skin and strange markings? They don’t seem human. Are they who I was just talking to before I was sucked into this world?

# Character: Alien 
~ monologue = false
- Why are you staring at me like that, like I’m some kind of $w(weirdo)? Who even are you anyways?

# Character: Innozen 
~ monologue = false
- It’s none of your business. I think I should be the one asking the questions here.

# Character: None
~ monologue = true
- Alien scoffs at you, annoyed with your cold demeanor.

# Character: Alien 
~ monologue = false 
- Huhhh? I’m pretty sure I was here before you, what gives you the right to ask me questions?

# Character: Innozen 
~ monologue = false 
- Well, why should I trust you or tell you anything? Weren’t you the bot I was just talking to?

# Character: Alien 
~ monologue = false 
- That was you!? You weren’t helpful at all!

 ~PLAY_MUSIC("Soft")
 
 # Character: None
~ monologue = true
 - The Alien turns away, giving the cold shoulder to you and starts to pout. When the Alien turns away, their foot presses down on the floor and part of it goes down, like the Alien stepped on a button. 
 
 - The ground starts to shake and a terminal rises out of the ground. You walks towards the terminal and presses the start button on the terminal’s interface. 
 
 - The terminal announces a message in a strange, mechanical voice: $s(“Would you like to continue?”)
 
  ~PLAY_MUSIC("Shocked")
 # Character: Alien 
~ monologue = false 
- Hey! This has never happened before, try it! Try it please! I’ve been stuck here, bored out of my mind. Maybe this can give us clues about how to leave this place.

# Character: None
~ monologue = true
- You stare at the terminal for a moment. 
- This is strange . . . But, I don’t know what else to do except push the button. 


 - A door appears with the name, Toto, inscribed on it. 

# Character: Innozen
~ monologue = false
- Who the heck is Toto?

# Character: Toto 
~ monologue = false
- $s(ME! THAT’S ME!) It knows who I am because I was here first.

~ monologue = true
- Both you and the Alien look at each other. You feel slightly annoyed. 

# Character: Toto 
~ monologue = false
- You know . . . Let’s just go through the door! Nothing to lose haha!

~ monologue = true
- You are at a loss for words but scrambled after Toto as the doors began to close.

# Character: Innozen 
~ monologue = true 
- Wait! Don’t be a $w(fool), think about this more!


-> END
VAR InkKnowledge = 0

-> This_Is_A_Knot
=== This_Is_A_Knot ===
This is a KNOT. Knots are kind of like functions and form the basic building blocks of an ink narrative. They operate like co-routine driven functions and allow for easy separation of story segments.
Writing in a knot requires no special formatting to appear before the player and can be easily read by the ink compiler.
~InkKnowledge += 1
Your Ink Knowledge is now {InkKnowledge}!
If you want to introduce choice, record decisions, trigger game events, or other interactive options formatting is required.
*This is a CHOICE -> DescribeChoicesA
*This is another CHOICE -> DescribeChoicesA
= gathertime
- This is a GATHER. It pulls the flow to a single point, regardless of previous choices.
New Knots can be entered at any time.
~InkKnowledge += 1
Your Ink Knowledge is now {InkKnowledge}!
* Try this one -> ThisOneA
* Or this one -> ThisOneA

=== ThisOneA ===
Knots can even flow -> ThisOneB

=== ThisOneB ===
mid-sentence.
This is the end of the demo, but there are a lot of other cool things you can do with Ink! Check out the documentation at github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md
If you want help integrating Ink into your project (reading it, etc.) go to github.com/inkle/ink/blob/master/Documentation/RunningYourInk.md
Or if you want to peer deep under the hood, go to github.com/inkle/ink/blob/master/Documentation/ArchitectureAndDevOverview.md
-> END

=== DescribeChoicesA ===
Choices lead to different lines. It's best to indent choice lines to show how they flow!
Any line opened with an asterisk becomes a choice. Multiple asterisks are used for nested levels of choices.
There are several ways to format choices based on what choice text you want to show the player.
This text is the same as if you had picked the other choice, but it doeesn't have to be.
~InkKnowledge += 1
Your Ink Knowledge is now {InkKnowledge}!
* [This is a choice]
	** [This is a sub-choice]
	** [This is also a sub-choice]
- -> DescribeChoicesB

=== DescribeChoicesB ===
Subchoices are supported infinitely, but each time a subchoice is given it requires an additional asterisk.
Notice how this time, you can't see the text for the option you picked? That's because I used BRACKETS [] to signify that the choice was read only during the choice prompt.
Any text that appears BEFORE these brackets will show up in both the choice and the printed line. Text INSIDE  the brackets appears on on the choice itself.
Any text that appears AFTER these brackets will show up in the printed line only.
Now we're going back to the first knot, but instead of the beginning we're going to a THREAD.
-> This_Is_A_Knot.gathertime
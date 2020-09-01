![Demo image](https://github.com/JanSkvaril/Exam-Timer/blob/master/demo.PNG?raw=true "Demo")
# Exam Timer
This is simple application for multistep exams that you can open on projector of monitor, so your students can easly see how mutch time they have left. 
## How to run
Files you need:
* **Timer.exe** - Main app
* **beep.wav** - Alarm sound (played when changing steps)
* **config.txt** - File containg all steps and timestamps 
## Config file
File **config.txt** must be in same directory as **Timer.exe**.

Format of **config.txt**:

        hh:mm
        text 1
        hh:mm 
        text 2

Text will be displayed until time on previous line

### Example

        8:00
        Introduction
        9:50
        Exam - Part A
        10:00
        Pause
        10:50
        Exam - Part B
        11:00
        Pause
        11:50
        Exam - Part C
        12:00
        Pause
        14:00
        Exam - Part D

If current time is 13:45, app will display *Exam - Part D* that will end at 14:00
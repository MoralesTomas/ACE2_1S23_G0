class Pomodoro {
  private:
    int workTime;
    int shortBreakTime;
    int longBreakTime;
    int completedPomodoros;
    bool isWorking;
    bool isLongBreak;
  
  public:
    Pomodoro(int wt, int sbt, int lbt) {
      workTime = wt;
      shortBreakTime = sbt;
      longBreakTime = lbt;
      completedPomodoros = 0;
      isWorking = false;
      isLongBreak = false;
    }

    void reset() {
      completedPomodoros = 0;
      isWorking = false;
      isLongBreak = false;
    }

    bool grupoCompleto() {
      return completedPomodoros == 4;
    }
    void startWork() {
      isWorking = true;
      isLongBreak = false;
    }

    void startShortBreak() {
      isWorking = false;
      isLongBreak = false;
      completedPomodoros++;
    }

    void startLongBreak() {
      isWorking = false;
      isLongBreak = true;
      completedPomodoros++;
    }

    bool isWorkingSession() {
      return isWorking;
    }

    bool isLongBreakSession() {
      return isLongBreak;
    }

    int getWorkTime() {
      return workTime;
    }

    int setWorkTime(int wt) {
      workTime = wt;
    }

    int getShortBreakTime() {
      return shortBreakTime;
    }

    int setShortBreakTime(int sbt) {
      shortBreakTime = sbt;
    }

    int getLongBreakTime() {
      return longBreakTime;
    }

    int setLongBreakTime(int lbt) {
      longBreakTime = lbt;
    }

    int getCompletedPomodoros() {
      return completedPomodoros;
    }

};

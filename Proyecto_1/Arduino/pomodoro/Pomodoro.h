#include "Song.h"

/**
 * @brief Object that represents a phisics pomodoro
 * 
 */
class Pomodoro {
  private:
    int workTime;
    int shortBreakTime;
    int longBreakTime;
    int completedPomodoros;
    bool isWorking;
    bool isBreak;
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

    void setAll(int wt, int sbt, int lbt) {
      workTime = wt;
      shortBreakTime = sbt;
      longBreakTime = lbt;
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
      startMelody();
      isWorking = false;
      isLongBreak = false;
      completedPomodoros++;
    }

    void startLongBreak() {
      startMelody();
      isWorking = false;
      isLongBreak = true;
      completedPomodoros++;
    }

    bool isWorkingSession() {
      return isWorking;
    }

    bool isBreakSession() {
      return isBreak;
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

    void stopWork() {
      isWorking = false;
    }

    void stopShortWork() {
      isWorking = false;
      isLongBreak = false;
      isBreak = false;
    }

    void stopLongWork() {
      isWorking = false;
      isLongBreak = false;
    }

    int getShortBreakTime() {
      return shortBreakTime;
    }

    int setShortBreakTime(int sbt) {
      shortBreakTime = sbt;
    }

    void stopBreak() {
      isWorking = true;
    }

    int getLongBreakTime() {
      return longBreakTime;
    }

    int setLongBreakTime(int lbt) {
      longBreakTime = lbt;
    }

    void stopLongBreak() {
      isWorking = true;
    }

    int getCompletedPomodoros() {
      return completedPomodoros;
    }

    void soundMelody() {
      startMelody();
    }
};

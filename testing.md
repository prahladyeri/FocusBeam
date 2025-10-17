## **1. Core Functionality Checks**

* [ ] **Task Management:** Create, edit, delete tasks/projects, assign priorities and deadlines.
* [ ] **Timer:** Start, pause, stop, and ensure duration is logged correctly in minutes and hours.
* [ ] **Time Entries:** Verify correct calculation and persistence of `TimeEntry` objects.
* [ ] **Data Saving/Loading:** Tasks and time entries persist correctly between app restarts.
* [ ] **Notifications & Tooltips:** Balloon tips and tooltips appear at the right moments.

---

## **2. UI/UX Checks**

* [ ] **Flicker-Free Updates:** Timers, labels, and dynamic panels don’t flicker (double-buffering confirmed).
* [ ] **Layout Responsiveness:** Resizing panels/windows doesn’t break layout or hide content.
* [ ] **Button States:** Start/Stop buttons toggle correctly with appropriate icons/text.
* [ ] **Readability:** Fonts/icons are clear; color contrasts are comfortable for long-term use.

---

## **3. Error Handling & Edge Cases**

* [ ] **Empty Task/Project:** Prevent adding blank or invalid tasks.
* [ ] **Invalid Time Entry:** Prevent negative durations or overlapping entries if applicable.
* [ ] **Timer Accuracy:** Ensure starting/stopping multiple times works correctly.
* [ ] **Exception Logging:** Any unexpected error should be caught and logged gracefully.

---

## **4. Performance & Stability**

* [ ] **CPU/Memory:** Timer updates, multiple tasks, or long sessions don’t spike CPU or memory usage.
* [ ] **Startup/Shutdown:** App launches and closes without delays or errors.
* [ ] **Long Usage Test:** Run for a few hours to ensure no crashes or memory leaks.

---

## **5. Compatibility**

* [ ] **Windows 10/11:** Confirm UI and notifications work as expected.
* [ ] **Different DPI/Resolutions:** UI scales correctly on 1080p, 1440p, etc.
* [ ] **Legacy Support (Optional):** If supporting older Windows, ensure basic functionality works.

---

## **6. Optional Polishing**

* [ ] **Export/Reports:** Export time entries to CSV or display totals correctly.
* [ ] **Sound Notifications:** Custom sounds play correctly when tracking starts/stops.
* [ ] **Tooltips:** Correct info displayed for tasks/timers.
* [ ] **Icons & Branding:** App looks professional for first user impression.
# TestsGenerator

Многопоточный генератор шаблонного кода тестовых классов для библиотеки тестирования NUnit. Генерируется по одному пустому тесту на каждый публичный метод тестируемого класса. Учитывается перегрузка методов.

**Входные данные:**
- список файлов для классов, из которых необходимо сгенерировать тестовые классы;
- путь к папке для записи созданных файлов;
- ограничения на секции конвейера.

**Выходные данные:**
- файлы с тестовыми классами.

**Генерация выполняется в конвейерном режиме и состоит из трех этапов:**
- параллельная загрузка исходных текстов в память (с ограничением количества файлов, загружаемых за раз);
- генерация тестовых классов в многопоточном режиме (с ограничением максимального количества одновременно обрабатываемых задач); 
- параллельная запись результатов на диск (с ограничением количества одновременно записываемых файлов).

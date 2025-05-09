// Файл car3DViewer.js
// Переменные для хранения основных объектов Three.js
let scene, camera, renderer, controls, model;
let container;

// Инициализация сцены
function init() {
  // Получаем контейнер для 3D-просмотра
  container = document.getElementById("car3DViewer");
  if (!container) return;

  // Создаем сцену
  scene = new THREE.Scene();
  scene.background = new THREE.Color(0xf5f5f5);

  // Создаем камеру
  const width = container.clientWidth;
  const height = container.clientHeight;
  camera = new THREE.PerspectiveCamera(45, width / height, 0.1, 1000);
  camera.position.set(5, 3, 10);

  // Создаем рендерер
  renderer = new THREE.WebGLRenderer({ antialias: true });
  renderer.setSize(width, height);
  renderer.setPixelRatio(window.devicePixelRatio);
  renderer.shadowMap.enabled = true;
  container.appendChild(renderer.domElement);

  // Добавляем освещение
  const ambientLight = new THREE.AmbientLight(0xffffff, 1.0);
  scene.add(ambientLight);

  // Направленный свет для теней
  const directionalLight = new THREE.DirectionalLight(0xffffff, 1.2);
  directionalLight.position.set(5, 10, 7.5);
  directionalLight.castShadow = true;
  scene.add(directionalLight);

  // Второй направленный свет с другой стороны
  const secondLight = new THREE.DirectionalLight(0xffffff, 0.8);
  secondLight.position.set(-5, 5, -7.5);
  scene.add(secondLight);

  // Добавляем нижний свет для подсветки днища
  const bottomLight = new THREE.DirectionalLight(0xffffff, 0.7);
  bottomLight.position.set(0, -5, 0);
  scene.add(bottomLight);

  // Добавляем передний свет для лучшего освещения передней части
  const frontLight = new THREE.DirectionalLight(0xffffff, 0.6);
  frontLight.position.set(0, 3, 15);
  scene.add(frontLight);

  // Добавляем OrbitControls для управления камерой
  controls = new THREE.OrbitControls(camera, renderer.domElement);
  controls.enableDamping = true;
  controls.dampingFactor = 0.05;
  controls.screenSpacePanning = false;
  controls.maxPolarAngle = Math.PI / 2;

  // Загружаем 3D модель
  loadModel();

  // Обработчик изменения размера окна
  window.addEventListener("resize", onWindowResize);

  // Запускаем анимацию
  animate();
}

// Загрузка 3D модели
function loadModel() {
  // Добавляем индикатор загрузки
  const loadingManager = new THREE.LoadingManager();
  const progressContainer = document.createElement("div");
  progressContainer.id = "loading-progress";
  progressContainer.style.position = "absolute";
  progressContainer.style.top = "50%";
  progressContainer.style.left = "50%";
  progressContainer.style.transform = "translate(-50%, -50%)";
  progressContainer.style.textAlign = "center";
  progressContainer.innerHTML =
    '<div>Загрузка модели...</div><div id="progress-percent">0%</div>';
  container.appendChild(progressContainer);

  loadingManager.onProgress = function (url, itemsLoaded, itemsTotal) {
    const percent = Math.round((itemsLoaded / itemsTotal) * 100);
    const progressElement = document.getElementById("progress-percent");
    if (progressElement) {
      progressElement.textContent = `${percent}%`;
    }
  };

  loadingManager.onLoad = function () {
    if (progressContainer && progressContainer.parentNode) {
      progressContainer.style.display = "none";
    }
  };

  // Создаем загрузчик GLTF
  const loader = new THREE.GLTFLoader(loadingManager);

  loader.load(
    "/models/scene.gltf",
    function (gltf) {
      model = gltf.scene;

      // Настройка модели
      model.traverse(function (node) {
        if (node.isMesh) {
          node.castShadow = true;
          node.receiveShadow = true;
        }
      });

      // Центрирование и масштабирование модели
      const box = new THREE.Box3().setFromObject(model);
      const center = box.getCenter(new THREE.Vector3());
      const size = box.getSize(new THREE.Vector3());

      // Перемещаем модель так, чтобы центр был в (0,0,0)
      model.position.x = -center.x;
      model.position.y = -center.y;
      model.position.z = -center.z;

      // Масштабируем модель, чтобы она хорошо вписывалась в видимую область
      const maxDim = Math.max(size.x, size.y, size.z);
      const scale = 5 / maxDim;
      model.scale.set(scale, scale, scale);

      // Добавляем модель на сцену
      scene.add(model);

      // Настраиваем камеру и контролы после загрузки модели
      controls.target.set(0, (size.y * scale) / 3, 0);
      controls.update();
    },
    // onProgress
    function (xhr) {
      // Прогресс загрузки обрабатывается через loadingManager
    },
    // onError
    function (error) {
      console.error("Ошибка при загрузке модели:", error);
      console.log("URL модели:", "/models/scene.gltf");
      console.log("Тип ошибки:", error.type);
      if (error.message) {
        console.log("Сообщение об ошибке:", error.message);
      }

      const progressElement = document.getElementById("loading-progress");
      if (progressElement) {
        progressElement.innerHTML =
          '<div class="error">Ошибка загрузки модели: ' +
          (error.message || "неизвестная ошибка") +
          "</div>";
      }

      // Пробуем загрузить резервную модель GLTF
      loadFallbackModel();
    }
  );
}

// Функция загрузки резервной модели
function loadFallbackModel() {
  console.log("Попытка загрузить резервную модель...");
  const loader = new THREE.GLTFLoader();
  loader.load(
    "/models/scene.gltf",
    function (gltf) {
      model = gltf.scene;

      // Настройка модели
      model.traverse(function (node) {
        if (node.isMesh) {
          node.castShadow = true;
          node.receiveShadow = true;
        }
      });

      // Центрирование и масштабирование модели
      const box = new THREE.Box3().setFromObject(model);
      const center = box.getCenter(new THREE.Vector3());
      const size = box.getSize(new THREE.Vector3());

      // Перемещаем модель так, чтобы центр был в (0,0,0)
      model.position.x = -center.x;
      model.position.y = -center.y;
      model.position.z = -center.z;

      // Масштабируем модель
      const maxDim = Math.max(size.x, size.y, size.z);
      const scale = 5 / maxDim;
      model.scale.set(scale, scale, scale);

      // Добавляем модель на сцену
      scene.add(model);

      // Настраиваем камеру и контролы после загрузки модели
      controls.target.set(0, (size.y * scale) / 3, 0);
      controls.update();

      console.log("Резервная модель успешно загружена");

      const progressElement = document.getElementById("loading-progress");
      if (progressElement && progressElement.parentNode) {
        progressElement.style.display = "none";
      }
    },
    // onProgress
    function (xhr) {
      // Прогресс загрузки
    },
    // onError
    function (error) {
      console.error("Ошибка при загрузке резервной модели:", error);
      const progressElement = document.getElementById("loading-progress");
      if (progressElement) {
        progressElement.innerHTML =
          '<div class="error">Не удалось загрузить ни одну модель. Пожалуйста, проверьте консоль для деталей.</div>';
      }
    }
  );
}

// Обработчик изменения размера окна
function onWindowResize() {
  if (!container) return;

  const width = container.clientWidth;
  const height = container.clientHeight;

  camera.aspect = width / height;
  camera.updateProjectionMatrix();
  renderer.setSize(width, height);
}

// Функция анимации
function animate() {
  requestAnimationFrame(animate);

  // Обновляем контролы
  if (controls) controls.update();

  // Рендеринг сцены
  if (renderer && scene && camera) {
    renderer.render(scene, camera);
  }
}

// Инициализация просмотра при загрузке страницы
document.addEventListener("DOMContentLoaded", function () {
  // Запускаем инициализацию с небольшой задержкой, чтобы DOM был полностью загружен
  setTimeout(init, 100);
});

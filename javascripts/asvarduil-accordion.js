var AsvarduilAccordion = function ($container) {
		// Class variables...
		var pub = {};
		pub.debugMode = false,
		pub.animateRate = 100,
		pub.hiddenClass = '.hidden',
		pub.headerClass = '.accordion-header',
		pub.contentClass = '.accordion-content',
		pub.$container = $container;
		
		// Public Scope
		pub.Activate = function () {
			$(pub.headerClass).click(pub.headerClass, function (e) {
				e.preventDefault();
				e.stopPropagation();
				
				var $self = $(this),
					$nextContent = $(this).next(pub.contentClass);
					
				hideVisibleContent();
				toggleContent($nextContent);
			});
		};
		
		// Private Scope
		function debugMessage(msg) {
			if(! pub.debugMode)
				return;
				
			if(console == null
			   || console == undefined)
			   return;
			   
			console.log(msg);
		}
		
		function hideVisibleContent() {
			$(pub.contentClass + ':visible').slideUp(pub.animateRate);
		}
		
		function toggleContent($content) {
			if(! $content.is(':visible')) {
				debugMessage('Showing next content...');
				$content.slideDown(pub.animateRate);
			}
			else {
				debugMessage('Hiding next content...');
				$content.slideUp(pub.animateRate);
			}
		}
		
		return pub;
};